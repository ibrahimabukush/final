using Microsoft.AspNetCore.Identity;

namespace eBook_Library_Service.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
