using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext _context;

        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Computed here (rather than inside the query) so Mongo only has to
            // compare dates, not perform date arithmetic itself.
            var overdueCutoff = DateTime.Today.AddDays(-14);

            ViewBag.BookCount = _context.Books.CountDocuments(FilterDefinition<Models.Book>.Empty);
            ViewBag.MemberCount = _context.Members.CountDocuments(FilterDefinition<Models.Member>.Empty);
            ViewBag.ActiveLoans = _context.Loans.CountDocuments(l => l.ReturnDate == null);
            ViewBag.OverdueLoans = _context.Loans.CountDocuments(
                l => l.ReturnDate == null && l.LoanDate < overdueCutoff);

            return View();
        }
    }
}
