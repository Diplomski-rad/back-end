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

            if (user != null && !user.IsActive)
            {
                throw new UserBannedException("User with given email is banned.");
            }

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

        public async Task<List<User>> GetAll()
        {
            try
            {
                var users = await _context.Users.Where(u => u.Role == UserRole.User || u.Role == UserRole.Author).ToListAsync();
                return users;


            }catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<long> AddAdmin(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user.Id;

            }catch(DbUpdateException ex)
            {
                throw;
            }
        }


        public async Task<long> BanUser(long userId)
        {
            try
            {
                var basicUser = await _context.BasicUsers.FirstOrDefaultAsync(bu => bu.Id == userId && bu.IsActive);
                if(basicUser == null)
                {
                    var author = await _context.Author.FirstOrDefaultAsync(a => a.Id == userId && a.IsActive);
                    
                    if(author == null)
                    {
                        throw new NotFoundException("User with given id don't exist.");
                    }

                    author.IsActive = false;

                    var courses = await _context.Course.Include(c => c.Author).Where(c => c.Author.Id == userId && c.Status == CourseStatus.PUBLISHED).ToListAsync();
                    foreach (var course in courses)
                    {
                        course.Status = CourseStatus.ARCHIVED;
                    }

                    await _context.SaveChangesAsync();

                    return author.Id;
                }
                else
                {
                    basicUser.IsActive = false;
                    var ratings = await _context.Rating.Where(r => r.UserId == userId).ToListAsync();
                    foreach (var rating in ratings)
                    {
                        rating.IsValid = false;
                    }

                    await _context.SaveChangesAsync();

                    return basicUser.Id;
                }
 
            }catch(DbUpdateException ex)
            {
                throw;
            }
        }

        public async Task<long> UnbanUser(long userId)
        {
            try
            {
                var basicUser = await _context.BasicUsers.FirstOrDefaultAsync(bu => bu.Id == userId && !bu.IsActive);
                if (basicUser == null)
                {
                    var author = await _context.Author.FirstOrDefaultAsync(a => a.Id == userId && !a.IsActive);

                    if (author == null)
                    {
                        throw new NotFoundException("User with given id don't exist.");
                    }

                    author.IsActive = true;

                    await _context.SaveChangesAsync();

                    return author.Id;
                }
                else
                {
                    basicUser.IsActive = true;
                    var ratings = await _context.Rating.Where(r => r.UserId == userId).ToListAsync();
                    foreach (var rating in ratings)
                    {
                        rating.IsValid = true;
                    }

                    await _context.SaveChangesAsync();

                    return basicUser.Id;
                }

            }
            catch (DbUpdateException ex)
            {
                throw;
            }
        }

    }
}
