using TrustedAbstractions;

namespace TrustedTools
{
    public static class BookExtensions
    {
        public static Book Clone(this Book book, int id)
        {
            return new Book
            {
                BookId = id,
                Title = book.Title,
                Genre = book.Genre,
                Price = book.Price,
                Author = book.Author,
                PublishDate = book.PublishDate,
            };
        }
    }
}
