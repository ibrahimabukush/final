using eBook_Library_Service.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBook_Library_Service.ViewModels;

namespace eBook_Library_Service.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Dashboard()
        {
            var totalBooks = _context.Books.Count();

            // Exclude the admin user from the total count
            var totalUsers = _context.Users.Count(u => u.Email != "admin@orig.il");

            var pendingBorrowRequests = _context.BorrowRequests.Count(r => r.Status == "Pending");

            var model = new AdminDashboardViewModel
            {
                TotalBooks = totalBooks,
                TotalUsers = totalUsers,
                PendingBorrowRequests = pendingBorrowRequests
            };

            return View(model);
        }
    }
}
