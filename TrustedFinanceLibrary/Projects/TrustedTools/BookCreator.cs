using TrustedAbstractions;

namespace TrustedTools
{
    public class BookCreator
    {
        public static Book CreateRandom(int id = -1)
        {
            return new Book
            {
                BookId = id,
                Title = BookTokenGenerator.RandomTitle(),
                Genre = BookTokenGenerator.RandomGenre(),
                Price = BookTokenGenerator.RandomPrice(),
                Author = BookTokenGenerator.RandomAuthor(),
                PublishDate = BookTokenGenerator.RandomPublishDate(),
            };
        }
    }
}
