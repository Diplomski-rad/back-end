using Courses_app.Models;

namespace Courses_app.Services
{
    public interface IUserService
    {
        public Task<User> AddBasicUser(BasicUser user);
        public Task<User> AddAuthor(Author user);
        public Task<User> GetByEmailAndPassword(string email, string password);
        public Task<BasicUser> GetBasicUser(long id);
    }
}
