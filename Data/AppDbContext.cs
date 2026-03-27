using Microsoft.EntityFrameworkCore;
using hw_2_2_3_26.Models;

namespace MyApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Cover> Covers { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(e =>
        {
            e.HasOne(e => e.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(e => e.PublisherId)
            .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Cover>(e =>
        {
            e.HasOne(c => c.Book)
            .WithMany(b => b.Covers)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookAuthor>(e =>
        {
            e.HasKey(e => new { e.BookId, e.AuthorId });

            e.HasOne(e => e.Book)
            .WithMany(a => a.BookAuthors)
            .HasForeignKey(e => e.BookId)
            .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(e => e.Author)
            .WithMany(a => a.BookAuthors)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookGenre>(e =>
        {
            e.HasKey(e => new { e.BookId, e.GenreId });

            e.HasOne(e => e.Book)
            .WithMany(a => a.BookGenres)
            .HasForeignKey(e => e.BookId)
            .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(e => e.Genre)
            .WithMany(a => a.BookGenres)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Author>(e =>
        {
            e.HasOne(e => e.Country)
            .WithMany(c => c.Authors)
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Publisher>(e =>
        {
            e.HasOne(e => e.Country)
            .WithMany(c => c.Publishers)
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.SetNull);
        });


        // SEEDING
        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "USA" },
            new Country { Id = 2, Name = "United Kingdom" },
            new Country { Id = 3, Name = "France" }
        );

        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "George", LastName = "Orwell", DateOfBirth = new DateTime(1903, 6, 25), CountryId = 2 },
            new Author { Id = 2, FirstName = "Stephen", LastName = "King", DateOfBirth = new DateTime(1947, 9, 21), CountryId = 1 },
            new Author { Id = 3, FirstName = "Victor", LastName = "Hugo", DateOfBirth = new DateTime(1802, 2, 26), CountryId = 3 },
            new Author { Id = 4, FirstName = "Agatha", LastName = "Christie", DateOfBirth = new DateTime(1890, 9, 15), CountryId = 2 },
            new Author { Id = 5, FirstName = "Neil", LastName = "Gaiman", DateOfBirth = new DateTime(1960, 11, 10), CountryId = 2 }
        );
        modelBuilder.Entity<Publisher>().HasData(
            new Publisher { Id = 1, Name = "Penguin Books", CountryId = 2, FoundedAt = new DateTime(1935, 7, 30) },
            new Publisher { Id = 2, Name = "HarperCollins", CountryId = 1, FoundedAt = new DateTime(1989, 8, 1) },
            new Publisher { Id = 3, Name = "Hachette Livre", CountryId = 3, FoundedAt = new DateTime(1826, 1, 1) }
        );

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Dystopian" },
            new Genre { Id = 2, Name = "Horror" },
            new Genre { Id = 3, Name = "Classic" },
            new Genre { Id = 4, Name = "Detective" },
            new Genre { Id = 5, Name = "Fantasy" },
            new Genre { Id = 6, Name = "Drama" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "1984", Year = 1949, PageCount = 328, PublisherId = 1, CreatedAt = new DateTime(2024, 1, 1) },
            new Book { Id = 2, Title = "Animal Farm", Year = 1945, PageCount = 112, PublisherId = 1, CreatedAt = new DateTime(2024, 1, 2) },
            new Book { Id = 3, Title = "The Shining", Year = 1977, PageCount = 447, PublisherId = 2, CreatedAt = new DateTime(2024, 1, 3) },
            new Book { Id = 4, Title = "It", Year = 1986, PageCount = 1138, PublisherId = 2, CreatedAt = new DateTime(2024, 1, 4) },
            new Book { Id = 5, Title = "Les Misérables", Year = 1862, PageCount = 1463, PublisherId = 3, CreatedAt = new DateTime(2024, 1, 5) },
            new Book { Id = 6, Title = "Murder on the Orient Express", Year = 1934, PageCount = 256, PublisherId = 1, CreatedAt = new DateTime(2024, 1, 6) },
            new Book { Id = 7, Title = "Good Omens", Year = 1990, PageCount = 288, PublisherId = 2, CreatedAt = new DateTime(2024, 1, 7) }
        );

        modelBuilder.Entity<BookAuthor>().HasData(
            // Orwell (2 books)
            new BookAuthor { BookId = 1, AuthorId = 1 },
            new BookAuthor { BookId = 2, AuthorId = 1 },

            // Stephen King (2 books)
            new BookAuthor { BookId = 3, AuthorId = 2 },
            new BookAuthor { BookId = 4, AuthorId = 2 },

            // Hugo
            new BookAuthor { BookId = 5, AuthorId = 3 },

            // Christie
            new BookAuthor { BookId = 6, AuthorId = 4 },

            new BookAuthor { BookId = 7, AuthorId = 5 },
            new BookAuthor { BookId = 7, AuthorId = 1 }
        );

        modelBuilder.Entity<BookGenre>().HasData(
            // 1984
            new BookGenre { BookId = 1, GenreId = 1 },
            new BookGenre { BookId = 1, GenreId = 6 },

            // Animal Farm
            new BookGenre { BookId = 2, GenreId = 1 },
            new BookGenre { BookId = 2, GenreId = 6 },

            // The Shining
            new BookGenre { BookId = 3, GenreId = 2 },
            new BookGenre { BookId = 3, GenreId = 6 },

            // IT
            new BookGenre { BookId = 4, GenreId = 2 },
            new BookGenre { BookId = 4, GenreId = 6 },

            // Les Misérables
            new BookGenre { BookId = 5, GenreId = 3 },
            new BookGenre { BookId = 5, GenreId = 6 },

            // Murder on the Orient Express
            new BookGenre { BookId = 6, GenreId = 4 },

            new BookGenre { BookId = 7, GenreId = 5 },
            new BookGenre { BookId = 7, GenreId = 6 }
        );
    }
}