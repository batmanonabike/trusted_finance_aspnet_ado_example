using TrustedAbstractions;

namespace TrustedWebApp.Services
{
    public class BookService(IBookRepository bookRepo) : IBookService
    {
        public List<Book> GetBooks() => bookRepo.ReadAll();
        public Book AddBook(Book book) => bookRepo.Add(book);
        public bool DeleteBook(int id) => bookRepo.Delete(id);
        public Book? GetBookById(int id) => bookRepo.Get(id);
        public bool UpdateBook(Book book) => bookRepo.Update(book);
    }

    public class LibraryService(ILibrary library) : ILibraryService
    {
        private readonly IBookService _bookService = new BookService(library.Books);
        public IBookService Books => _bookService;
    }
}
