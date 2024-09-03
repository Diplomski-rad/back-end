using BCrypt.Net;
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
            user.Password = EncodePassword(user.Password);
            return await _repository.AddBasicUser(user);
        }
        public async Task<User> AddAuthor(Author user)
        {
            user.Password = EncodePassword(user.Password);
            return await _repository.AddAuthor(user);
        }

        public async Task<User> GetByEmailAndPassword(string email, string password)
        {
            var user = await _repository.GetByEmail(email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password,user.Password))
            {
                return user;
            }

            return null;
        }

        public Task<BasicUser> GetBasicUser(long id)
        {
            return _repository.GetBasicUser(id);
        }

        private string EncodePassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
