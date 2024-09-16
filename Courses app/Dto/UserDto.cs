using Courses_app.Models;

namespace Courses_app.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public UserDto()
        {
            
        }

        public UserDto(User user)
        {
            Id = user.Id;
            Email = user.Email;
            Username = user.Username;
            Role = user.Role.ToString();
            IsActive = user.IsActive;
        }
    }
}
