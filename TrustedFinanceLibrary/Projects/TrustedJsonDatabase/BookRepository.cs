using TrustedTools;
using TrustedAbstractions;
using TrustedJsonDatabase.Helpers;

namespace TrustedJsonDatabase
{
    public class BookRepository(LibrarySettings settings) : IBookRepository
    {
        private readonly JsonStore _jsonStore = new(settings.JsonLibrary);

        public int Count() => _jsonStore.Query(content => content.Books.Count);

        public List<Book> ReadAll()
        {
            return _jsonStore.Query(content => content.Books.ToList());
        }

        public Book? Get(int id)
        {
            return _jsonStore.Query(content =>
            {
                return content.Books.FirstOrDefault(book => book.BookId == id);
            });
        }

        public bool Exists(int id)
        {
            return _jsonStore.Query(content =>
            {
                return content.Books.Any(book => book.BookId == id);
            });
        }

        public Book Add(Book book)
        {
            return _jsonStore.Modify(content =>
            {
                var createdBook = book.Clone(GetNextId(content));
                content.Books.Add(createdBook);
                return createdBook;
            });
        }

        public bool Update(Book book)
        {
            return _jsonStore.Modify(content =>
            {
                var index = content.Books.FindIndex(existingBook => existingBook.BookId == book.BookId);
                if (index < 0) return false;

                content.Books[index] = book;
                return true;
            });
        }

        public bool Delete(int id)
        {
            return _jsonStore.Modify(content =>
            {
                var index = content.Books.FindIndex(existing => existing.BookId == id);
                if (index < 0) return false;

                content.Books.RemoveAt(index);
                return true;
            });
        }

        private static int GetNextId(JsonContent content)
        {
            if (content.Books.Count == 0)
                return 1;

            return content.Books.Max(book => book.BookId) + 1;
        }
    }
}
