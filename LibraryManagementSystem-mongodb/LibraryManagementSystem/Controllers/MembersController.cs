using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LibraryManagementSystem.Controllers
{
    public class MembersController : Controller
    {
        private readonly LibraryContext _context;

        public MembersController(LibraryContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var members = _context.Members.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                members = members.Where(m =>
                    m.Name.Contains(search) ||
                    m.Email.Contains(search));
            }

            ViewBag.Search = search;
            return View(members.OrderBy(m => m.Name).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Member member)
        {
            if (ModelState.IsValid)
            {
                member.Id = _context.GetNextId("Members");
                _context.Members.InsertOne(member);

                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        public IActionResult Edit(int id)
        {
            var member = _context.Members.Find(m => m.Id == id).FirstOrDefault();

            if (member == null)
                return NotFound();

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Member member)
        {
            if (id != member.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Members.ReplaceOne(m => m.Id == id, member);

                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        public IActionResult Delete(int id)
        {
            var member = _context.Members.Find(m => m.Id == id).FirstOrDefault();

            if (member == null)
                return NotFound();

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var member = _context.Members.Find(m => m.Id == id).FirstOrDefault();

            if (member == null)
                return NotFound();

            if (_context.Loans.Find(l => l.MemberId == id).Any())
            {
                TempData["Error"] = "This member has loan history and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Members.DeleteOne(m => m.Id == id);

            return RedirectToAction(nameof(Index));
        }
    }
}
