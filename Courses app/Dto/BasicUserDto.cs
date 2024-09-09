using Courses_app.Models;

namespace Courses_app.Dto
{
    public class BasicUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }   

        public BasicUserDto(BasicUser user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Username = user.Username;
            Email = user.Email;
        }
    }
}
