using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBook_Library_Service.Controllers
{
    public class BookController : Controller
    {
        private Repository<Book> books;
        private Repository<Author> authors;
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public BookController(AppDbContext context, IWebHostEnvironment webHostingEnvironment)
        {
            books = new Repository<Book>(context);
            authors = new Repository<Author>(context);
            _webHostingEnvironment = webHostingEnvironment;
        }


        public async Task<IActionResult> Index(string category = null)
        {
            IEnumerable<Book> bookList;

            if (string.IsNullOrEmpty(category))
            {
                bookList = await books.GetAllsync();
            }
            else
            {
                bookList = await books.GetAllsync();
                bookList = bookList.Where(b => b.Category == category);
            }

            ViewBag.Categories = await GetCategories();
            ViewBag.SelectedCategory = category;

            return View(bookList);
        }

        private async Task<IEnumerable<string>> GetCategories()
        {
            var categories = await books.GetAllsync();
            return categories.Select(b => b.Category).Distinct().ToList();
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {

            ViewBag.Authors = await authors.GetAllsync();

            if (id == 0)
            {
                ViewBag.operation = "Add";
                return View(new Book());
            }

            var book = await books.GetByIdAsync(id, new QueryOptions<Book>
            {
                Includes = "BookAuthors.Author",
                Where = b => b.BookId == id
            });

            if (book == null)
            {
                return NotFound();
            }

            ViewBag.operation = "Edit";
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Book book, int[] authorIds)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("stops heereeeeeeeeeeee22");
                // Default image URL if no image is uploaded
                string defaultImagePath = "images/BookDefult.png";

                ViewBag.Authors = await authors.GetAllsync();

                // Handle image upload (only if a new image is uploaded)
                if (book.ImageFile != null)
                {
                    string uploads = Path.Combine(_webHostingEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + book.ImageFile.FileName;
                    string filePath = Path.Combine(uploads, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }

                    book.ImageUrl = uniqueFileName;  // Update the ImageUrl with the new image
                }
                else
                {
                    // If no image is uploaded, use the default image
                    book.ImageUrl = defaultImagePath;
                }

                if (book.BookId == 0)  // Add new book
                {
                    foreach (int id in authorIds)
                    {
                        book.BookAuthors.Add(new BookAuthor { AuthorId = id });
                    }

                    try
                    {
                        await books.AddAsync(book);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.GetBaseException().Message);
                    }
                }
             
                else  // Edit existing book
                {
                    Console.WriteLine("stops heereeeeeeeeeeee22");
                    var existingBook = await books.GetByIdAsync(book.BookId, new QueryOptions<Book> { Includes = "BookAuthors" });

                    if (existingBook == null)
                    {
                        ModelState.AddModelError("", "Book not found");
                        return View(book);
                    }
                    Console.WriteLine("stops heereeeeeeeeeeee22");

                    // Update other properties
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

                    // If the image URL was updated (new image uploaded), update it
                    if (!string.IsNullOrEmpty(book.ImageUrl))
                    {
                        Console.WriteLine(book.ImageUrl);
                        Console.WriteLine("stops heereeeeeeeeeeee");
                        existingBook.ImageUrl = book.ImageUrl;
                    }

                    // Update authors
                    existingBook.BookAuthors.Clear();
                    foreach (int id in authorIds)
                    {
                        existingBook.BookAuthors.Add(new BookAuthor { AuthorId = id });
                    }

                    try
                    {
                        await books.UpdateAsync(existingBook);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.GetBaseException().Message);
                        return View(book);
                    }
                }
            }

            return View(book);
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await books.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Book not found");
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> UserIndex(string category = null, string searchQuery = null)
        {
            var bookList = await books.GetAllsync();

            foreach (var book in bookList)
            {
                if (book.DiscountPrice.HasValue && book.DiscountPrice.Value > 0 && book.BuyPrice > 0)
                {
                    // Calculate the discounted price
                    var discountAmount = book.BuyPrice * (book.DiscountPrice.Value / 100);
                    book.BuyPrice = book.BuyPrice - discountAmount; // Update the BuyPrice with the discounted price
                }
                var bookWithAuthors = await books.GetByIdAsync(book.BookId, new QueryOptions<Book>
                {
                    Includes = "BookAuthors.Author"
                });
                book.BookAuthors = bookWithAuthors.BookAuthors;
            }

            // Apply category filter if specified
            if (!string.IsNullOrEmpty(category))
            {
                bookList = bookList.Where(b => b.Category != null && b.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Apply search query filter if specified
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

            if (IsAjaxRequest(Request))
            {
                return PartialView("_BookList", bookList);
            }

            return View(bookList);
        }

        public async Task<IActionResult> SearchBook(string category = null, string searchIndex = null)
        {
            var bookList = await books.GetAllsync();

            foreach (var book in bookList)

            {
                if (book.DiscountPrice.HasValue && book.DiscountPrice.Value > 0 && book.BuyPrice > 0)
                {
                    // Calculate the discounted price
                    var discountAmount = book.BuyPrice * (book.DiscountPrice.Value / 100);
                    book.BuyPrice = book.BuyPrice - discountAmount; // Update the BuyPrice with the discounted price
                }
                var bookWithAuthors = await books.GetByIdAsync(book.BookId, new QueryOptions<Book>
                {
                    Includes = "BookAuthors.Author"
                });
                book.BookAuthors = bookWithAuthors.BookAuthors;
            }

            // Apply category filter if specified
            if (!string.IsNullOrEmpty(category))
            {
                bookList = bookList.Where(b => b.Category != null && b.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Apply search query filter if specified
            if (!string.IsNullOrEmpty(searchIndex))
            {
                bookList = bookList.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.Contains(searchIndex, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.Description) && b.Description.Contains(searchIndex, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.Category) && b.Category.Contains(searchIndex, StringComparison.OrdinalIgnoreCase)));
            }

            ViewBag.Categories = await GetCategories();
            ViewBag.SelectedCategory = category;
            ViewBag.SearchQuery = searchIndex;

            return PartialView("_BookList", bookList);
        }


        public bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await books.GetByIdAsync(id, new QueryOptions<Book>
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


    }
}
