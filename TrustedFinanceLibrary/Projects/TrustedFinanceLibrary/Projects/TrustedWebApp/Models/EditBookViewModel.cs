using TrustedAbstractions;
using System.ComponentModel.DataAnnotations;

namespace TrustedWebApp.Models
{
    public class EditBookViewModel
    {
        public int BookId { get; init; }

        [Required] public DateOnly PublishDate { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Author { get; set; } = string.Empty;
        [Required] public string Genre { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue)] public decimal Price { get; set; }

        public Book ToBook()
        {
            return new()
            {
                Title = Title,
                Price = Price,
                Genre = Genre,
                Author = Author,
                BookId = BookId,
                PublishDate = PublishDate,
            };
        }

        public static EditBookViewModel From(Book book)
        {
            return new()
            {
                Title = book.Title,
                Price = book.Price,
                Genre = book.Genre,
                Author = book.Author,
                BookId = book.BookId,
                PublishDate = book.PublishDate,
            };
        }
    }
}
