using System.Net;
using System.Net.Http.Json;
using TrustedTools;
using TrustedAbstractions;
using TrustedTests.Helpers;

namespace TrustedTests.WebAppTests
{
    [Collection(TestGroupNames.WebApp)]
    public class BooksControllerTests(TrustedWebAppFactory factory) : IClassFixture<TrustedWebAppFactory>, IAsyncLifetime
    {
        private readonly List<int> _createdBookIds = [];
        private readonly HttpClient _client = factory.CreateClient();

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            foreach (var id in _createdBookIds)
                await _client.DeleteAsync($"/api/books/{id}");
        }

        private async Task<Book> AddBook()
        {
            var book = BookCreator.CreateRandom();
            var response = await _client.PostAsJsonAsync("/api/books", book);
            response.EnsureSuccessStatusCode();

            var createdBook = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(createdBook);
            _createdBookIds.Add(createdBook.BookId);

            return createdBook;
        }

        [Fact]
        public async Task GetBooks_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/books");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddBook_ReturnsBook()
        {
            var createdBook = await AddBook();
            Assert.True(createdBook.BookId > 0);
        }

        [Fact]
        public async Task GetBookById_ReturnsBook_WhenBookExists()
        {
            var createdBook = await AddBook();
            var response = await _client.GetAsync($"/api/books/{createdBook.BookId}");
            var fetchedBook = await response.Content.ReadFromJsonAsync<Book>();

            Assert.NotNull(fetchedBook);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(createdBook.EqualsByValue(fetchedBook));
        }

        [Fact]
        public async Task GetBookById_ReturnsNotFound_WhenBookMissing()
        {
            var response = await _client.GetAsync("/api/books/-1");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNoContent_WhenBookExists()
        {
            var createdBook = await AddBook();
            var updatedBook = BookCreator.CreateRandom(createdBook.BookId);
            var response = await _client.PutAsJsonAsync($"/api/books/{createdBook.BookId}", updatedBook);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBook_ReturnsBadRequest_WhenIdMismatched()
        {
            var createdBook = await AddBook();
            var response = await _client.PutAsJsonAsync($"/api/books/{createdBook.BookId + 1}", createdBook);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookExists()
        {
            var createdBook = await AddBook();
            var response = await _client.DeleteAsync($"/api/books/{createdBook.BookId}");
            _createdBookIds.Remove(createdBook.BookId);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookMissing()
        {
            var response = await _client.DeleteAsync("/api/books/-1");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
