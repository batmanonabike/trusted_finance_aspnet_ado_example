namespace TrustedAbstractions
{
    public class Book // record not permitted by spec.
    {
        public int BookId { get; init; } = 0;
        public required string Title { get; init; }
        public required string Author { get; init; }
        public required string Genre { get; init; }
        public required decimal Price { get; init; }
        public required DateOnly PublishDate { get; init; }
    }

    public interface IBookRepository
    {
        int Count();
        List<Book> ReadAll();
        bool Exists(int id);

        Book? Get(int id);
        Book Add(Book book);
        bool Delete(int id);
        bool Update(Book book);
    }

    public interface ILibrary : IDisposable
    {
        bool IsOpen();
        IBookRepository Books { get; }
    }

    public interface IBookService
    {
        List<Book> GetBooks();
        Book AddBook(Book book);
        bool DeleteBook(int id);
        bool UpdateBook(Book book);
        Book? GetBookById(int id);
    }

    public interface ILibraryService
    { 
        IBookService Books { get; }
    }
}
