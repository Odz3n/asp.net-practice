namespace hw_2_2_3_26.Services;

public enum ContentType
{
    Books,
    Comics,
    Authors,
    Publishers,
    Magazines
}
public enum SubContentType
{
    Covers,
    Photos,
    Samples,
    Thumbnails
}
public class FileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadsRoot;
    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        _uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }

    /// <summary>
    /// Saves a file for any content type (books, comics, authors, etc.)
    /// </summary>
    /// <param name="contentType">The type of content (Books, Comics, Authors, etc.)</param>
    /// <param name="targetId">The ID of the entity</param>
    /// <param name="subContentType">The subfolder type (Covers, Photos, etc.)</param>
    /// <param name="file">The file to save</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Relative path to the saved file</returns>
    public async Task<string?> SaveFileAsync(
        ContentType contentType,
        int targetId,
        SubContentType subContentType,
        IFormFile file,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File was not provided.");

        var contentTypeName = contentType.ToString().ToLowerInvariant();
        var subContentTypeName = subContentType.ToString().ToLowerInvariant();

        // Create folder structure: uploads/{contentType}/{id}/{subContentType}/
        var contentFolder = Path.Combine(_uploadsRoot, contentTypeName, $"{targetId}");
        var targetFolder = Path.Combine(contentFolder, subContentTypeName);

        Directory.CreateDirectory(targetFolder);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{subContentTypeName}_{timestamp}_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(targetFolder, fileName);

        await using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream, ct);
        }

        return Path.Combine("uploads", contentTypeName, $"{targetId}", subContentTypeName, fileName)
            .Replace('\\', '/');
    }

    /// <summary>
    /// Convenience method for saving book covers
    /// </summary>
    public async Task<string?> SaveBookCoverAsync(int bookId, IFormFile file, CancellationToken ct)
    {
        return await SaveFileAsync(ContentType.Books, bookId, SubContentType.Covers, file, ct);
    }
    /// <summary>
    /// Deletes a file by its relative path
    /// </summary>
    public async Task<bool> DeleteFile(string relativePath, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(relativePath))
            return false;

        var normalizedRelativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace("uploads\\", "");
        Console.WriteLine(normalizedRelativePath);
        
        var fullPath = Path.Combine(_uploadsRoot, normalizedRelativePath);
        Console.WriteLine(fullPath);
        

        if (!File.Exists(fullPath))
            return false;

        await Task.Run(() => File.Delete(fullPath), ct);
        return true;
    }
    /// <summary>
    /// Deletes an entire entity directory (all files for a book, author, etc.)
    /// </summary>
    public async Task<bool> DeleteEntityDirectoryAsync(
        ContentType contentType,
        int entityId,
        CancellationToken ct)
    {
        var contentTypeName = contentType.ToString().ToLowerInvariant();
        var entityFolder = Path.Combine(_uploadsRoot, contentTypeName, $"{entityId}");
        if (!Directory.Exists(entityFolder))
            return false;

        await Task.Run(() => Directory.Delete(entityFolder, true), ct);
        return true;
    }
    private string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title))
            return "untitled";

        var slug = title.ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');

        if (slug.Length > 50)
            slug = slug[..50];

        return slug;
    }
}