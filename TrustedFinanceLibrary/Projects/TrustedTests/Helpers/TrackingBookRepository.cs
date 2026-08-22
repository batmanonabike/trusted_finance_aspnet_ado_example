using TrustedAbstractions;

namespace TrustedTests.Helpers
{
    internal class TrackingBookRepository(IBookRepository inner, IIdTracker idTracker) : IBookRepository
    {
        public bool Exists(int id)
        {
            return inner.Exists(id);
        }

        public int Count()
        {
            return inner.Count();
        }

        public Book Add(Book book)
        {
            var result = inner.Add(book);
            idTracker.AddId(result.BookId);
            return result;
        }

        public bool Delete(int id)
        {
            var result = inner.Delete(id);
            if (result) idTracker.RemoveId(id);
            return result;
        }

        public List<Book> ReadAll()
        {
            return inner.ReadAll();
        }

        public Book? Get(int id)
        {
            return inner.Get(id);
        }

        public bool Update(Book book)
        {
            return inner.Update(book);
        }
    }
}
