using Courses_app.Models;
using Courses_app.Repository;

namespace Courses_app.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<User> AddBasicUser(BasicUser user)
        {
            return await _repository.AddBasicUser(user);
        }
        public async Task<User> AddAuthor(Author user)
        {
            return await _repository.AddAuthor(user);
        }

        public async Task<User> GetByEmailAndPassword(string email, string password)
        {
            return await _repository.GetByEmailAndPassword(email, password);
        }

        public Task<BasicUser> GetBasicUser(long id)
        {
            return _repository.GetBasicUser(id);
        }
    }
}
