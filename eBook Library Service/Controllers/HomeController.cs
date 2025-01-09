using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using eBook_Library_Service.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace eBook_Library_Service.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context; // Add this field

        // Inject UserManager<Users> and AppDbContext via constructor
        public HomeController(
            ILogger<HomeController> logger,
            UserManager<Users> userManager,
            AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context; // Initialize _context
        }

        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var submission = new Contact
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message
                };

                _context.Contacts.Add(submission);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Your message has been sent successfully!";
                return RedirectToAction("Index");
            }

            // If the form is not valid, return to the contact page with validation errors.
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Populate the ViewModel
            var model = new UserProfileViewModel
            {
                FullName = user.FullName, // Use FullName
                Email = user.Email,
                // Populate other fields as needed
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties
                user.FullName = model.FullName; // Use FullName
                // Update other fields as needed

                // Save changes
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Profile updated successfully!";
                    return RedirectToAction("Profile");
                }

                // Add errors to ModelState if the update fails
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If the model state is invalid, return the view with errors
            return View(model);
        }
    }
}