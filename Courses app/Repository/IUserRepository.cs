using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface IUserRepository
    {
        public Task<User> AddBasicUser(BasicUser user);
        public Task<User> AddAuthor(Author user);
        public Task<User> GetByEmail(string email);
        public Task<Author> GetAuthorById(long id);
        public Task<BasicUser> GetBasicUser(long id);
    }
}
