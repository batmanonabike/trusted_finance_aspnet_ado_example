using Microsoft.AspNetCore.Mvc;
using TrustedAbstractions;

namespace TrustedWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(ILibraryService library) : ControllerBase
    {
        // GET /api/books
        [HttpGet]
        [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
        public ActionResult<List<Book>> GetBooks()
        {
            return Ok(library.Books.GetBooks());
        }

        // POST /api/books
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Book> AddBook([FromBody] Book book)
        {
            var addedBook = library.Books.AddBook(book);
            return CreatedAtAction(nameof(GetBookById), new { id = addedBook.BookId }, addedBook);
        }

        // DELETE /api/books/42
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteBook(int id)
        {
            return library.Books.DeleteBook(id) ? NoContent() : NotFound();
        }

        // PUT /api/books/42
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.BookId)
                return BadRequest();

            return library.Books.UpdateBook(book) ? NoContent() : NotFound();
        }

        // GET /api/books/42
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        public ActionResult<Book> GetBookById(int id)
        {
            var book = library.Books.GetBookById(id);
            return (book == null) ? NotFound() : Ok(book);
        }
    }
}
