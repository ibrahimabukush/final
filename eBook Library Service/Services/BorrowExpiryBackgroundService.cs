using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using eBook_Library_Service.Services;

public class BorrowExpiryBackgroundService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _services;
    private readonly ILogger<BorrowExpiryBackgroundService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BorrowExpiryBackgroundService(
        IServiceProvider services,
        ILogger<BorrowExpiryBackgroundService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _services = services;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Borrow Expiry Background Service is starting.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Check every 10 seconds
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        _logger.LogInformation("Borrow Expiry Background Service is working.");
        using (var scope = _services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Find expired borrows
            var expiredBorrows = await dbContext.BorrowHistory
                .Where(b => b.ReturnDate <= DateTime.Now && b.ReturnDate != DateTime.MinValue)
                .ToListAsync();

            foreach (var borrow in expiredBorrows)
            {
                try
                {
                    // Update book stock
                    var book = await dbContext.Books.FindAsync(borrow.BookId);
                    if (book != null)
                    {
                        book.Stock += 1;
                        _logger.LogInformation($"Increased stock for book ID {borrow.BookId}.");
                    }

                    // Remove the borrow record
                    dbContext.BorrowHistory.Remove(borrow);
                    _logger.LogInformation($"Removed borrow record ID {borrow.BorrowId}.");

                    // Notify the next user in the waiting list
                    var waitingList = await dbContext.WaitingLists
                        .Where(w => w.BookId == borrow.BookId)
                        .OrderBy(w => w.Position)
                        .ToListAsync();

                    if (waitingList.Any())
                    {
                        var nextUser = waitingList.First();

                        // Assign the book to the next user in the waiting list
                        var newBorrow = new BorrowHistory
                        {
                            UserId = nextUser.UserId,
                            BookId = nextUser.BookId,
                            BorrowDate = DateTime.Now,
                            ReturnDate = DateTime.Now.AddMinutes(10) // Set the return date (e.g., 10 minutes from now)
                        };

                        dbContext.BorrowHistory.Add(newBorrow);
                        _logger.LogInformation($"Assigned book ID {borrow.BookId} to user {nextUser.UserId}.");

                        // Remove the user from the waiting list
                        dbContext.WaitingLists.Remove(nextUser);
                        _logger.LogInformation($"Removed user {nextUser.UserId} from the waiting list.");

                        // Update positions of remaining users
                        foreach (var entry in waitingList.Skip(1))
                        {
                            entry.Position -= 1;
                        }

                        // Send email notification to the next user
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                        var user = await dbContext.Users.FindAsync(nextUser.UserId);

                        if (book != null && user != null)
                        {
                            // Generate the payment link
                            var paymentLink = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Payment/ProcessBorrowPayment?bookId={book.BookId}&userId={user.Id}";

                            var emailSubject = "Book Available for Borrowing - Payment Required";
                            var emailMessage = $"Dear {user.FullName},<br/><br/>" +
                                               $"The book <strong>{book.Title}</strong> is now available for borrowing.<br/>" +
                                               $"To complete your borrowing request, please click the link below to make the payment:<br/><br/>" +
                                               $"<a href='{paymentLink}'>Pay Now</a><br/><br/>" +
                                               $"You have 24 hours to complete the payment. After payment, the book will be added to your Borrow History.<br/><br/>" +
                                               $"Thank you for using eBook Library Service!<br/><br/>" +
                                               $"Best regards,<br/>" +
                                               $"eBook Library Service Team";

                            try
                            {
                                await emailService.SendEmailAsync(user.Email, emailSubject, emailMessage);
                                _logger.LogInformation($"Notification email sent to {user.Email}.");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Failed to send notification email to {user.Email}: {ex.Message}");
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing borrow record ID {borrow.BorrowId}.");
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Borrow Expiry Background Service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}