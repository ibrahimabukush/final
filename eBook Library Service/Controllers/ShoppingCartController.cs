using eBook_Library_Service.Data;
using eBook_Library_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eBook_Library_Service.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCartService _shoppingCartService;
        private readonly AppDbContext _context;
        public ShoppingCartController(ShoppingCartService shoppingCartService, AppDbContext context)
        {
            _shoppingCartService = shoppingCartService;
            _context = context;
        }

        // View the shopping cart
        public async Task<IActionResult> Index()
        {
            var cart = await _shoppingCartService.GetCartAsync();
            return View(cart);
        }

        // Add an item to the cart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int bookId, bool isForBorrow)
        {
            try
            {
                await _shoppingCartService.AddToCartAsync(bookId, isForBorrow);
                return RedirectToAction("Index"); // Redirect to the cart page
            }
            catch (Exception ex)
            {
                // Log the error and return to the details page with an error message
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Book", new { id = bookId });
            }
        }
        // Remove an item from the cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int shoppingCartItemId)
        {
            await _shoppingCartService.RemoveFromCartAsync(shoppingCartItemId);
            return RedirectToAction("Index");
        }

        // Clear the cart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            await _shoppingCartService.ClearCartAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var cart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefaultAsync(sc => sc.UserId == userId);

            var count = cart?.Items.Sum(item => item.Quantity) ?? 0;
            return Json(new { count });
        }

        public async Task<IActionResult> PurchaseHistory()
        {
            // Get the current user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fetch the user's purchase history
            var purchases = await _context.PurchaseHistories
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return View(purchases);
        }
    }
}