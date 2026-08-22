using TrustedAbstractions;

namespace TrustedTests.Helpers
{
    internal static class BookExtensions
    {
        public static bool EqualsByValue(this Book book1, Book book2)
        {
            return BookRecord.From(book1) == BookRecord.From(book2);
        }
    }

    // Localised for equality in the tests.  The spec asks for a class.
    public record BookRecord
    {
        public int BookId { get; init; } = 0;
        public required string Title { get; init; }
        public required string Author { get; init; }
        public required string Genre { get; init; }
        public required decimal Price { get; init; }
        public required DateOnly PublishDate { get; init; }

        public static BookRecord From(Book book)
        {
            return new BookRecord
            {
                Title = book.Title,
                Genre = book.Genre,
                Price = book.Price,
                BookId = book.BookId,
                Author = book.Author,
                PublishDate = book.PublishDate,
            };
        }
    }
}
