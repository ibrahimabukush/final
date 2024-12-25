using System.ComponentModel.DataAnnotations;

namespace eBook_Library_Service.ViewModels
{
    public class ChangePassworViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be {2} and at max {1} charaters long .")]
        [DataType(DataType.Password)]
        [Display(Name = " New Password")]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords do not match.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
