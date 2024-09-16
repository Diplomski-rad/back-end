using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface IUserService
    {
        public Task<User> AddBasicUser(BasicUser user);
        public Task<User> AddAuthor(Author user);
        public Task<User> GetByEmailAndPassword(string email, string password);
        public Task<BasicUserDto> GetBasicUser(long id);
        public Task<AuthorDetailsDto> GetAuthor(long id);
        public Task<User> ChangePassword(long id, ChangePassordRequest request);
        public Task<User> UpdateUser(long userId, UpdateUserRequest request, UserRole role);
        public Task<List<UserDto>> GetAll();
        public Task<List<UserDto>> Search(string query, SearchUserFlag flag);
        public Task<long> AddAdmin(User user);
        public Task<long> BanUser(long userId);
        public Task<long> UnbanUser(long userId);
    }
}
