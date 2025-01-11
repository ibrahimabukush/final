using Microsoft.AspNetCore.Mvc;
using eBook_Library_Service.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using eBook_Library_Service.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using eBook_Library_Service.Data;
using PayPal.Api;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace eBook_Library_Service.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayPalService _payPalService;
        private readonly StripeService _stripeService;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public PaymentController(
            PayPalService payPalService,
            StripeService stripeService,
            ShoppingCartService shoppingCartService,
            ILogger<PaymentController> logger,
            IConfiguration configuration,
            AppDbContext context,
            EmailService emailService)
        {
            _payPalService = payPalService;
            _stripeService = stripeService;
            _shoppingCartService = shoppingCartService;
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }

        // GET: ProcessBorrowPayment (for email notification link)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ProcessBorrowPayment(int bookId)
        {
            // Fetch the book details
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToAction("UserIndex", "Book");
            }

            // Pass the book details and payment options to the view
            ViewBag.BookId = bookId;
            ViewBag.BookTitle = book.Title;
            ViewBag.BorrowPrice = book.BorrowPrice;

            return View("ChoosePaymentMethod"); // Render a view to choose payment method
        }

        // POST: ProcessBorrowPayment (for form submission)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProcessBorrowPayment(int bookId, string paymentMethod)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (userId == null)
                {
                    return Unauthorized(); // User is not logged in
                }

                // Fetch the book details
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    TempData["ErrorMessage"] = "Book not found.";
                    return RedirectToAction("UserIndex", "Book");
                }

                // Check if the user has already borrowed this book
                var isAlreadyBorrowed = await _context.BorrowHistory
                    .AnyAsync(bh => bh.UserId == userId && bh.BookId == bookId);

                if (isAlreadyBorrowed)
                {
                    TempData["ErrorMessage"] = "You have already borrowed this book.";
                    return RedirectToAction("UserIndex", "Book");
                }

                // Check if the user has already borrowed 3 books
                var borrowedBooksCount = await _context.BorrowHistory
                    .CountAsync(bh => bh.UserId == userId && bh.ReturnDate > DateTime.UtcNow);

                if (borrowedBooksCount >= 3)
                {
                    TempData["ErrorMessage"] = "You have reached the maximum borrowing limit of 3 books.";
                    return RedirectToAction("UserIndex", "Book");
                }

                // If the book is available, proceed with the payment
                if (book.Stock > 0)
                {
                    if (paymentMethod == "paypal")
                    {
                        // Handle PayPal payment
                        var returnUrl = Url.Action("BorrowSuccess", "Payment", new { bookId = bookId }, Request.Scheme);
                        var cancelUrl = Url.Action("PaymentCancel", "Payment", null, Request.Scheme);

                        var payment = _payPalService.CreatePayment(book.BorrowPrice, returnUrl, cancelUrl, "sale");
                        return Redirect(payment.GetApprovalUrl());
                    }
                    else if (paymentMethod == "stripe")
                    {
                        // Handle Stripe payment
                        var clientSecret = _stripeService.CreatePaymentIntent(book.BorrowPrice);

                        ViewBag.StripeClientSecret = clientSecret;
                        ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
                        ViewBag.BookId = bookId;
                        ViewBag.IsBorrow = true; // Set this flag to indicate a borrow payment

                        return View("~/Views/ShoppingCart/StripeCheckout.cshtml");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid payment method selected.";
                        return RedirectToAction("UserIndex", "Book");
                    }
                }
                else
                {
                    // If the book is not available, set TempData to show the waiting list dialog
                    TempData["ShowWaitingListPopup"] = true;
                    TempData["BookId"] = bookId;
                    TempData["BookTitle"] = book.Title;
                    return RedirectToAction("UserIndex", "Book");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the borrow payment process.");
                TempData["ErrorMessage"] = "An error occurred during the borrow payment process. Please try again.";
                return RedirectToAction("UserIndex", "Book");
            }
        }

        [Authorize]
        public async Task<IActionResult> BorrowSuccess(int bookId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (userId == null)
                {
                    return Unauthorized(); // User is not logged in
                }

                // Fetch the book details
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    TempData["ErrorMessage"] = "Book not found.";
                    return RedirectToAction("UserIndex", "Book");
                }

                // Reduce the book's stock
                book.Stock -= 1;

                // Add the borrow record to the BorrowHistory table
                var borrowHistory = new BorrowHistory
                {
                    UserId = userId,
                    BookId = bookId,
                    BorrowDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddMinutes(10) // 30-day borrowing period
                };

                _context.BorrowHistory.Add(borrowHistory);
                await _context.SaveChangesAsync();

                // Send a success email
                var emailSubject = "Borrow Successful";
                var emailMessage = $"Dear {User.Identity.Name},<br/><br/>" +
                                   $"You have successfully borrowed the book <strong>{book.Title}</strong>.<br/>" +
                                   $"You can now access the book in your library.<br/><br/>" +
                                   $"Thank you for using eBook Library Service!<br/><br/>" +
                                   $"Best regards,<br/>" +
                                   $"eBook Library Service Team";

                await _emailService.SendEmailAsync(userEmail, emailSubject, emailMessage);
                _logger.LogInformation($"Borrow confirmation email sent to {userEmail}.");

                // Set success message
                TempData["Message"] = $"You have successfully borrowed {book.Title}.";

                // Redirect to the BorrowHistory page
                return RedirectToAction("BorrowHistory", "Book");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the borrow success process.");
                TempData["ErrorMessage"] = "An error occurred during the borrow success process. Please contact support.";
                return RedirectToAction("UserIndex", "Book");
            }
        }
    }
}