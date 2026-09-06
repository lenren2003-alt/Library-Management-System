using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // Show all books and allow searching.
        public IActionResult Index(string search)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();

                books = books.Where(b =>
                    b.Title.ToLower().Contains(term) ||
                    b.Author.ToLower().Contains(term) ||
                    b.ISBN.ToLower().Contains(term));
            }

            ViewBag.Search = search;
            return View(books.OrderBy(b => b.Title).ToList());
        }

        // Show the Add Book form.
        public IActionResult Create()
        {
            return View();
        }

        // Receive the Add Book form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                book.Id = _context.GetNextId("Books");
                _context.Books.InsertOne(book);

                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // Show the Edit form.
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(b => b.Id == id).FirstOrDefault();

            if (book == null)
                return NotFound();

            return View(book);
        }

        // Save edited book.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book book)
        {
            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Books.ReplaceOne(b => b.Id == id, book);

                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // Show delete confirmation.
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(b => b.Id == id).FirstOrDefault();

            if (book == null)
                return NotFound();

            return View(book);
        }

        // Delete the book.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _context.Books.Find(b => b.Id == id).FirstOrDefault();

            if (book == null)
                return NotFound();

            if (_context.Loans.Find(l => l.BookId == id).Any())         //Do not delete a book that has loan history.
            {
                TempData["Error"] = "This book has loan history and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Books.DeleteOne(b => b.Id == id);

            return RedirectToAction(nameof(Index));
        }
    }
}
