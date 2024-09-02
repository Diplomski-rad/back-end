using Courses_app.Models;

namespace Courses_app.Dto
{
    public class AuthorDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }

        public AuthorDto(Author author)
        {
            Name = author.Name;
            Surname = author.Surname;
            Username = author.Username;
        }
    }

}
