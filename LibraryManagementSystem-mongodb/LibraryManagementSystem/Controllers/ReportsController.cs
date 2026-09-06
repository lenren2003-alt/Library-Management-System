using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LibraryManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        // Loans are due back 14 days after they're borrowed.
        // (Matches the rule already used on the Home and Loans pages.)
        private const int LoanPeriodDays = 14;

        private readonly LibraryContext _context;

        public ReportsController(LibraryContext context)
        {
            _context = context;
        }

        // Landing page linking to each report.
        public IActionResult Index()
        {
            var cutoff = DateTime.Today.AddDays(-LoanPeriodDays);

            ViewBag.OverdueCount = _context.Loans.Find(l =>
                l.ReturnDate == null && l.LoanDate < cutoff).CountDocuments();

            return View();
        }

        // Every loan that is still out and past its due date.
        public IActionResult Overdue()
        {
            var cutoff = DateTime.Today.AddDays(-LoanPeriodDays);

            var loans = _context.Loans.AsQueryable()
                .Where(l => l.ReturnDate == null && l.LoanDate < cutoff)
                .OrderBy(l => l.LoanDate)
                .ToList();

            var books = _context.Books.AsQueryable().ToList().ToDictionary(b => b.Id);
            var members = _context.Members.AsQueryable().ToList().ToDictionary(m => m.Id);

            var overdue = loans.Select(l => new OverdueLoanViewModel
            {
                LoanId = l.Id,
                BookTitle = books.GetValueOrDefault(l.BookId)?.Title ?? "Unknown",
                MemberName = members.GetValueOrDefault(l.MemberId)?.Name ?? "Unknown",
                MemberEmail = members.GetValueOrDefault(l.MemberId)?.Email ?? "",
                LoanDate = l.LoanDate,
                DueDate = l.LoanDate.AddDays(LoanPeriodDays)
            }).ToList();

            return View(overdue);
        }

        // The 10 books borrowed the most times, all-time.
        // (Grouping/joining across collections is done here in memory, since
        // MongoDB collections aren't relational tables you can join server-side.)
        public IActionResult PopularBooks()
        {
            var loans = _context.Loans.AsQueryable().ToList();
            var books = _context.Books.AsQueryable().ToList().ToDictionary(b => b.Id);

            var popular = loans
                .GroupBy(l => l.BookId)
                .Select(g => new PopularBookViewModel
                {
                    Title = books.GetValueOrDefault(g.Key)?.Title ?? "Unknown",
                    Author = books.GetValueOrDefault(g.Key)?.Author ?? "",
                    BorrowCount = g.Count()
                })
                .OrderByDescending(b => b.BorrowCount)
                .ThenBy(b => b.Title)
                .Take(10)
                .ToList();

            return View(popular);
        }

        // The 10 members who have borrowed the most books, all-time.
        public IActionResult FrequentBorrowers()
        {
            var loans = _context.Loans.AsQueryable().ToList();
            var members = _context.Members.AsQueryable().ToList().ToDictionary(m => m.Id);

            var frequent = loans
                .GroupBy(l => l.MemberId)
                .Select(g => new FrequentBorrowerViewModel
                {
                    Name = members.GetValueOrDefault(g.Key)?.Name ?? "Unknown",
                    Email = members.GetValueOrDefault(g.Key)?.Email ?? "",
                    BorrowCount = g.Count()
                })
                .OrderByDescending(m => m.BorrowCount)
                .ThenBy(m => m.Name)
                .Take(10)
                .ToList();

            return View(frequent);
        }
    }
}
