using TrustedTools;
using TrustedAbstractions;
using Xunit.Abstractions;

namespace TrustedTests.Helpers
{
    public abstract class BookTester(ILibraryFactory libraryFactory, ITestOutputHelper output)
    {
        private ILibrary CreateLibrary() => libraryFactory.Create(output.WriteLine);

        [Fact]
        public void CanConnect()
        {
            using var library = CreateLibrary();
            Assert.True(library.IsOpen());
        }

        [Fact]
        public void CanCountBooks()
        {
            using var library = CreateLibrary();
            Assert.True(library.Books.Count() >= 0);
        }

        [Fact]
        public void CanCreateBook()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom();

            var createdBook = library.Books.Add(generatedBook);
            Assert.True(createdBook.BookId > 0);
        }

        [Fact]
        public void CanReadBook()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom(-1);

            var createdBook = library.Books.Add(generatedBook);
            Assert.True(createdBook.BookId > 0);

            var retrievedBook = library.Books.Get(createdBook.BookId);
            Assert.NotNull(retrievedBook);
            Assert.True(createdBook.EqualsByValue(retrievedBook));
        }

        [Fact]
        public void CanReadAllBooks()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom();

            var createdBook = library.Books.Add(generatedBook);
            Assert.True(createdBook.BookId > 0);

            int bookCount = library.Books.Count();
            var books = library.Books.ReadAll();
            Assert.True(books.Count > 0);
            Assert.Equal(bookCount, books.Count);
        }

        [Fact]
        public void CanUpdateBook()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom();

            var createdBook = library.Books.Add(generatedBook);
            Assert.True(createdBook.BookId > 0);

            var bookModified = BookCreator.CreateRandom(createdBook.BookId);
            Assert.True(library.Books.Update(bookModified));

            var bookUpdated = library.Books.Get(createdBook.BookId);
            Assert.NotNull(bookUpdated);
            Assert.True(bookModified.EqualsByValue(bookUpdated));
        }

        [Fact]
        public void ShouldNotUpdateBook()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom();
            Assert.False(library.Books.Update(generatedBook));
        }

        [Fact]
        public void CanDeleteBook()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom();
            int bookCount = library.Books.Count();

            var createdBook = library.Books.Add(generatedBook);
            Assert.True(createdBook.BookId > 0);

            Assert.Equal(bookCount + 1, library.Books.Count());
            Assert.True(library.Books.Delete(createdBook.BookId));
            Assert.Null(library.Books.Get(createdBook.BookId));
            Assert.Equal(bookCount, library.Books.Count());
        }

        [Fact]
        public void ShouldNotDeleteBook()
        {
            using var library = CreateLibrary();
            Assert.False(library.Books.Delete(-1));
        }

        [Fact]
        public void BookShouldExist()
        {
            using var library = CreateLibrary();
            var generatedBook = BookCreator.CreateRandom();

            var createdBook = library.Books.Add(generatedBook);
            Assert.True(createdBook.BookId > 0);
            Assert.True(library.Books.Exists(createdBook.BookId));
        }

        [Fact]
        public void BookShouldNotExist()
        {
            using var library = CreateLibrary();
            Assert.False(library.Books.Exists(-1));
        }
    }
}
