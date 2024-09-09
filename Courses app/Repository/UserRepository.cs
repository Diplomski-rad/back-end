using Courses_app.Exceptions;
using Courses_app.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Courses_app.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CoursesAppDbContext _context;

        public UserRepository(CoursesAppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Get(long id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
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

        public async Task<User> GetByEmail(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            return user;
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

        public async Task<User> ChangePassword(long id, string newPassword)
        {
            try
            {
                var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

                if(user == null)
                {
                    return null;
                }

                user.Password = newPassword;

                await _context.SaveChangesAsync();
                return user;

            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }
        }



        public async Task<User> UpdateUser(long id, string name, string surname, string username, UserRole role)
        {
            try
            {
                if(role == UserRole.Author)
                {
                    var user = await _context.Author.FirstOrDefaultAsync(u => u.Id == id);
                    if (user == null)
                    {
                        return null;
                    }

                    if (name != null) { user.Name = name; }
                    if (surname != null) { user.Surname = surname; }
                    if (username != null) { user.Username = username; }

                    await _context.SaveChangesAsync();
                    return user;

                }else if(role == UserRole.User)
                {
                    var user = await _context.BasicUsers.FirstOrDefaultAsync(u => u.Id == id);
                    if (user == null)
                    {
                        return null;
                    }

                    if (name != null) { user.Name = name; }
                    if (surname != null) { user.Surname = surname; }
                    if (username != null) { user.Username = username; }

                    await _context.SaveChangesAsync();
                    return user;
                }
                else
                {
                    return null;
                }
                

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresEx && postgresEx.SqlState == "23505")
                {
                    throw new UsernameAlreadyExistsException("The username is already in use. Please choose a different username.");
                }

                throw;
            }
        }

    }
}
