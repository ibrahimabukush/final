using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBook_Library_Service.Controllers
{
    public class BookController : Controller
    {
        private readonly Repository<Book> _books;
        private readonly Repository<Author> _authors;
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public BookController(AppDbContext context, IWebHostEnvironment webHostingEnvironment)
        {
            _books = new Repository<Book>(context);
            _authors = new Repository<Author>(context);
            _webHostingEnvironment = webHostingEnvironment;
        }

        // Admin Index Page
        public async Task<IActionResult> Index(string category = null)
        {
            var bookList = await _books.GetAllsync();

            if (!string.IsNullOrEmpty(category))
            {
                bookList = bookList.Where(b => b.Category == category);
            }

            ViewBag.Categories = await GetCategories();
            ViewBag.SelectedCategory = category;

            return View(bookList);
        }

        // User Index Page
        public async Task<IActionResult> UserIndex(string category = null, string searchQuery = null)
        {
            var bookList = await _books.GetAllsync();

            // Apply discounts
            foreach (var book in bookList)
            {
                if (book.DiscountPrice.HasValue && book.DiscountPrice.Value > 0 && book.BuyPrice > 0)
                {
                    var discountAmount = book.BuyPrice * (book.DiscountPrice.Value / 100);
                    book.BuyPrice -= discountAmount;
                }

                var bookWithAuthors = await _books.GetByIdAsync(book.BookId, new QueryOptions<Book>
                {
                    Includes = "BookAuthors.Author"
                });
                book.BookAuthors = bookWithAuthors.BookAuthors;
            }

            // Apply filters
            if (!string.IsNullOrEmpty(category))
            {
                bookList = bookList.Where(b => b.Category == category);
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                bookList = bookList.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.Description) && b.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.Category) && b.Category.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
            }

            ViewBag.Categories = await GetCategories();
            ViewBag.SelectedCategory = category;
            ViewBag.SearchQuery = searchQuery;

            // Return partial view for AJAX requests
            if (IsAjaxRequest(Request))
            {
                return PartialView("_BookList", bookList);
            }

            return View(bookList);
        }
        // Add/Edit Book (Admin)
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Authors = await _authors.GetAllsync();

            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Book());
            }

            var book = await _books.GetByIdAsync(id, new QueryOptions<Book>
            {
                Includes = "BookAuthors.Author",
                Where = b => b.BookId == id
            });

            if (book == null)
            {
                return NotFound();
            }

            ViewBag.Operation = "Edit";
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Book book, int[] authorIds)
        {
            if (ModelState.IsValid)
            {
                string defaultImagePath = "images/BookDefult.png";

                // Handle image upload
                if (book.ImageFile != null)
                {
                    string uploads = Path.Combine(_webHostingEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + book.ImageFile.FileName;
                    string filePath = Path.Combine(uploads, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }

                    book.ImageUrl = uniqueFileName;
                }
                else
                {
                    book.ImageUrl = defaultImagePath;
                }

                if (book.BookId == 0) // Add new book
                {
                    foreach (int id in authorIds)
                    {
                        book.BookAuthors.Add(new BookAuthor { AuthorId = id });
                    }

                    await _books.AddAsync(book);
                }
                else // Edit existing book
                {
                    var existingBook = await _books.GetByIdAsync(book.BookId, new QueryOptions<Book> { Includes = "BookAuthors" });

                    if (existingBook == null)
                    {
                        return NotFound();
                    }

                    // Update properties
                    existingBook.Title = book.Title;
                    existingBook.Publisher = book.Publisher;
                    existingBook.Description = book.Description;
                    existingBook.YearPublished = book.YearPublished;
                    existingBook.BuyPrice = book.BuyPrice;
                    existingBook.BorrowPrice = book.BorrowPrice;
                    existingBook.DiscountPrice = book.DiscountPrice;
                    existingBook.DiscountEndDate = book.DiscountEndDate;
                    existingBook.Stock = book.Stock;
                    existingBook.AgeLimit = book.AgeLimit;
                    existingBook.Category = book.Category;

                    // Update image URL if changed
                    if (!string.IsNullOrEmpty(book.ImageUrl))
                    {
                        existingBook.ImageUrl = book.ImageUrl;
                    }

                    // Update authors
                    existingBook.BookAuthors.Clear();
                    foreach (int id in authorIds)
                    {
                        existingBook.BookAuthors.Add(new BookAuthor { AuthorId = id });
                    }

                    await _books.UpdateAsync(existingBook);
                }

                return RedirectToAction("Index");
            }

            ViewBag.Authors = await _authors.GetAllsync();
            return View(book);
        }

        // Delete Book (Admin)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _books.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Book not found");
                return RedirectToAction("Index");
            }
        }

        // Book Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _books.GetByIdAsync(id, new QueryOptions<Book>
            {
                Includes = "BookAuthors.Author",
                Where = b => b.BookId == id
            });

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // Helper Methods
        private async Task<IEnumerable<string>> GetCategories()
        {
            var categories = await _books.GetAllsync();
            return categories.Select(b => b.Category).Distinct().ToList();
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}