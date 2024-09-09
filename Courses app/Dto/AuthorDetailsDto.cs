using Courses_app.Models;

namespace Courses_app.Dto
{
    public class AuthorDetailsDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string PayPalEmail { get; set; }
        public string Email { get; set; }   

        public AuthorDetailsDto(Author author)
        {
            Name = author.Name;
            Surname = author.Surname;
            Username = author.Username;
            Email = author.Email;
            PayPalEmail = author.PayPalEmail;
        }
    }
}
