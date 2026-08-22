using Microsoft.AspNetCore.Mvc;
using TrustedTools;
using TrustedAbstractions;
using TrustedWebApp.Models;

namespace TrustedWebApp.Controllers
{
    public class LibraryController(ILibraryService library) : Controller
    {
        public IActionResult Index()
        {
            var books = library.Books.GetBooks();
            return View(books);
        }

        [HttpPost]
        public IActionResult PopulateTestData()
        {
            for (int i = 0; i < 5; i++)
            {
                var book = BookCreator.CreateRandom();
                library.Books.AddBook(book);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            if (!library.Books.DeleteBook(id))
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = library.Books.GetBookById(id);
            if (book == null) 
                return NotFound();
            
            return View(EditBookViewModel.From(book));
        }

        [HttpPost]
        public IActionResult Edit(EditBookViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var book = viewModel.ToBook();
            if (!library.Books.UpdateBook(book))
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
