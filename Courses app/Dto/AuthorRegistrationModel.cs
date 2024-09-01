using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class AuthorRegistrationModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PayPalEmail { get; set; }
    }
}
