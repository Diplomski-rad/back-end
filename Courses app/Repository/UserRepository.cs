using Courses_app.Exceptions;
using Courses_app.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses_app.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CoursesAppDbContext _context;

        public UserRepository(CoursesAppDbContext context)
        {
            _context = context;
        }
        public async Task<User> AddBasicUser(BasicUser user)
        {
            try
            {
                _context.BasicUsers.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key value violates unique constraint \"IX_Users_Email\""))
                {
                    throw new EmailAlreadyExistsException("The email is already in use. Please choose another email.");
                }
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key value violates unique constraint \"IX_Users_Username\""))
                {
                    throw new UsernameAlreadyExistsException("The username is already in use. Please choose a different username.");
                }
                

                // Re-throw the exception if it's not related to the unique constraint
                throw;
            } 
        }

        public async Task<User> AddAuthor(Author user)
        {
            try
            {
                _context.Author.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key value violates unique constraint \"IX_Users_Email\""))
                {
                    throw new EmailAlreadyExistsException("The email is already in use. Please choose another email.");
                }
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key value violates unique constraint \"IX_Users_Username\""))
                {
                    throw new UsernameAlreadyExistsException("The username is already in use. Please choose a different username.");
                }


                // Re-throw the exception if it's not related to the unique constraint
                throw;
            }
        }

        public async Task<User> GetByEmailAndPassword(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && user.Password == password)
            {
                return user;
            }

            return null;
        }

        public async Task<Author> GetAuthorById(long id)
        {

            try
            {
                var user = await _context.Author
                 .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new NotFoundException($"Author not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving the course.", ex);
            }

        }

        public async Task<BasicUser> GetBasicUser(long id)
        {
            try
            {
                var user = await _context.BasicUsers
                .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving the course.", ex);
            }
            
        }

    }
}
