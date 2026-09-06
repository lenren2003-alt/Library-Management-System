using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LibraryManagementSystem.Controllers
{
    public class LoansController : Controller
    {
        private readonly LibraryContext _context;

        public LoansController(LibraryContext context)
        {
            _context = context;
        }

        // Show current and previous loans.
        public IActionResult Index()
        {
            var loans = _context.Loans.AsQueryable()
                .OrderByDescending(l => l.LoanDate)
                .ToList();

            AttachBooksAndMembers(loans);

            return View(loans);
        }

        // Show the Borrow Book form.
        public IActionResult Create()
        {
            ViewBag.Books = _context.Books.AsQueryable()
                .Where(b => b.IsAvailable)
                .OrderBy(b => b.Title)
                .ToList();

            ViewBag.Members = _context.Members.AsQueryable()
                .OrderBy(m => m.Name)
                .ToList();

            return View();
        }

        // Save a new loan.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int bookId, int memberId)
        {
            var book = _context.Books.Find(b => b.Id == bookId).FirstOrDefault();
            var member = _context.Members.Find(m => m.Id == memberId).FirstOrDefault();

            if (book == null || member == null)
            {
                TempData["Error"] = "Please choose a valid book and member.";
                return RedirectToAction(nameof(Create));
            }

            if (!book.IsAvailable)
            {
                TempData["Error"] = "This book is already borrowed.";
                return RedirectToAction(nameof(Create));
            }

            var loan = new Loan
            {
                Id = _context.GetNextId("Loans"),
                BookId = bookId,
                MemberId = memberId,
                LoanDate = DateTime.Today
            };

            book.IsAvailable = false;

            _context.Loans.InsertOne(loan);
            _context.Books.ReplaceOne(b => b.Id == book.Id, book);

            return RedirectToAction(nameof(Index));
        }

        // Return a borrowed book.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(int id)
        {
            var loan = _context.Loans.Find(l => l.Id == id).FirstOrDefault();

            if (loan == null)
                return NotFound();

            if (loan.ReturnDate != null)
                return RedirectToAction(nameof(Index));

            loan.ReturnDate = DateTime.Today;
            _context.Loans.ReplaceOne(l => l.Id == id, loan);

            var book = _context.Books.Find(b => b.Id == loan.BookId).FirstOrDefault();
            if (book != null)
            {
                book.IsAvailable = true;
                _context.Books.ReplaceOne(b => b.Id == book.Id, book);
            }

            return RedirectToAction(nameof(Index));
        }

        // MongoDB stores Loans, Books, and Members as separate collections with
        // no server-side joins, so we fetch the related Books/Members ourselves
        // and attach them in memory (this replaces EF's .Include(...) calls).
        private void AttachBooksAndMembers(List<Loan> loans)
        {
            var bookIds = loans.Select(l => l.BookId).Distinct().ToList();
            var memberIds = loans.Select(l => l.MemberId).Distinct().ToList();

            var books = _context.Books.Find(b => bookIds.Contains(b.Id))
                .ToList()
                .ToDictionary(b => b.Id);

            var members = _context.Members.Find(m => memberIds.Contains(m.Id))
                .ToList()
                .ToDictionary(m => m.Id);

            foreach (var loan in loans)
            {
                loan.Book = books.GetValueOrDefault(loan.BookId);
                loan.Member = members.GetValueOrDefault(loan.MemberId);
            }
        }
    }
}
