using eBook_Library_Service.Data;
using eBook_Library_Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBook_Library_Service.Controllers
{
    public class UserController : Controller
    {
        //public const string SessionKeyName = "Username";
        public const string SessionKeyEmail = "UserEmail";
        //public const string SessionKeyPhone = "UserPhone";
        //public const string SessionAddress = "UserAddress";

        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return _context.User != null ?
                        View(await _context.User.ToListAsync()) :
                        Problem("Entity set 'ASMContext.User'  is null.");
        }

        [HttpGet("/profile")]
        public IActionResult profile()
        {
            string userEmail = HttpContext.Session.GetString("UserEmail");
            var user = _context.User.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return Redirect("/login");
            }
            return View("Profile", user);
        }

        //[HttpPost("/profile")]
        //public async Task<IActionResult> updateProfile(User updatedUser)
        //{
        //    String userEmail = HttpContext.Session.GetString(SessionKeyEmail);
        //    if (userEmail == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userEmail);
        //    user.Name = updatedUser.Name;
        //    user.Email = updatedUser.Email;
        //    user.Address = updatedUser.Address;
        //    user.PhoneNumber = updatedUser.PhoneNumber;

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(profile));
        //}


    }
}
