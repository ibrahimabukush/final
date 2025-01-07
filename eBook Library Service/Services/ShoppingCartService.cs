using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eBook_Library_Service.Services
{
    public class ShoppingCartService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Get the current user's shopping cart
        public async Task<ShoppingCart> GetCartAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception("User not logged in.");
            }

            var cart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .ThenInclude(sci => sci.Book)
                .FirstOrDefaultAsync(sc => sc.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // Add an item to the cart
        public async Task AddToCartAsync(int bookId, bool isForBorrow)
        {
            var cart = await GetCartAsync();
            var existingItem = cart.Items.FirstOrDefault(sci => sci.BookId == bookId && sci.IsForBorrow == isForBorrow);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Items.Add(new ShoppingCartItem
                {
                    BookId = bookId,
                    IsForBorrow = isForBorrow,
                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync();
        }

        // Remove an item from the cart
        public async Task RemoveFromCartAsync(int shoppingCartItemId)
        {
            var cart = await GetCartAsync();
            var item = cart.Items.FirstOrDefault(sci => sci.ShoppingCartItemId == shoppingCartItemId);

            if (item != null)
            {
                cart.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // Clear the cart
        public async Task ClearCartAsync()
        {
            var cart = await GetCartAsync();
            if (cart != null)
            {
                cart.Items.Clear(); // Remove all items from the cart
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }
    }
}