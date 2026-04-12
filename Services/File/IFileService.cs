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
public interface IFileService
{
    Task<string?> SaveFileAsync(
        ContentType contentType,
        int targetId,
        SubContentType subContentType,
        IFormFile file,
        CancellationToken ct);

    Task<bool> DeleteFile(string relativePath, CancellationToken ct);

    Task<bool> DeleteEntityDirectoryAsync(
        ContentType contentType,
        int entityId,
        CancellationToken ct);
}