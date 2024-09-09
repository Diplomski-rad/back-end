using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface IUserRepository
    {
        public Task<User> Get(long id);
        public Task<User> AddBasicUser(BasicUser user);
        public Task<User> AddAuthor(Author user);
        public Task<User> GetByEmail(string email);
        public Task<Author> GetAuthorById(long id);
        public Task<BasicUser> GetBasicUser(long id);
        public Task<User> ChangePassword(long id, string newPassword);
        public Task<User> UpdateUser(long id, string name, string surname, string username, UserRole role);
    }
}
