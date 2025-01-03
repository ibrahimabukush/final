using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace eBook_Library_Service.Controllers
{
    public class AuthorController : Controller
    {

        private Repository<Author> authors;
        public AuthorController(AppDbContext context)
        {
            authors = new Repository<Author>(context);
        }
        public async Task<IActionResult> Index()
        {
            return View(await authors.GetAllsync());
        }
        public async Task<IActionResult> Details(int id)
        {
            return View(await authors.GetByIdAsync(id, new QueryOptions<Author>() { Includes = "BookAuthors.Book" }));
        }
    
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId, Name")] Author author)
        {
            // Debug: Log or inspect the incoming author object
            if (author == null)
            {
                Console.WriteLine("Author object is null.");
                return View(author);
            }

            Console.WriteLine($"Author Name: {author.Name}");

            if (ModelState.IsValid)
            {
                await authors.AddAsync(author);
                Console.WriteLine("Author added successfully.");
                return RedirectToAction("Index");
            }

            // Debug: Log ModelState errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Error: {error.ErrorMessage}");
            }

            Console.WriteLine("ModelState is not valid.");
            return View(author);
        }
        //Author Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(await authors.GetByIdAsync(id,new QueryOptions<Author> { Includes= "BookAuthors.Book"}));
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Author author)
        {
            await authors.DeleteAsync(author.AuthorId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await authors.GetByIdAsync(id, new QueryOptions<Author> { Includes = "BookAuthors.Book" }));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                await authors.UpdateAsync(author);
                return RedirectToAction("Index");
            }
            return View(author);
        }
    }
}
