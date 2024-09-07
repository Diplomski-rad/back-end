using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Courses_app.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CoursesAppDbContext _context;

        private readonly ICategoryRepository _categoryRepository;

        public CourseRepository(CoursesAppDbContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;

        }
        public async Task<long> Add(Course course)
        {
            try
            {
                _context.Course.Add(course);
                await _context.SaveChangesAsync();
                return course.Id;
                
            }
            catch (DbUpdateException ex)
            {
                throw;
            }
        }

        public async Task<Course> Get(long id)
        {
            try
            {
                var course = await _context.Course
                    .Include(c => c.Author)   
                    .Include(c => c.Videos)
                    .Include(c => c.Categories)
                    .Include(c=>c.Ratings)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (course == null)
                {
                    throw new NotFoundException($"Course with id {id} not found.");
                }

                return course;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while retrieving the course.", ex);
            }
        }

        public async Task<Course> GetAuthorCourse(long authorId, long courseId)
        {
            try
            {
                var course = await _context.Course
                    .Include(c => c.Author)
                    .Include(c => c.Videos)
                    .Include(c => c.Categories)
                    .Include(c => c.Ratings)
                    .FirstOrDefaultAsync(c => c.Id == courseId && c.Author.Id == authorId);

                return course;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while retrieving the course.", ex);
            }
        }

        public async Task<List<Course>> GetCoursesByIds(List<long> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    throw new ArgumentException("The list of IDs cannot be null or empty.");
                }

                var courses = await _context.Course
                    .Where(c => ids.Contains(c.Id)) 
                    .Include(c => c.Author)               
                    .ToListAsync();                 

                if (courses.Count == 0)
                {
                    throw new NotFoundException("No courses found for the given IDs.");
                }

                return courses;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while retrieving the courses.", ex);
            }
        }

        public async Task<List<Course>> GetAuthorCourses(long authorId)
        {
            List<Course> courses = await _context.Course
                .Include(c => c.Author)
                .Include(c => c.Videos)
                .Include(c => c.Categories)
                .Include(c => c.Ratings)
                .Where(c => c.Author.Id == authorId).ToListAsync();
            return courses;
        }

        public async Task<List<Video>> GetCourseVideos(long courseId)
        {
            var course = await _context.Course
               .Include(c => c.Videos)
               .FirstOrDefaultAsync(c => c.Id == courseId);

            return course?.Videos.ToList() ?? new List<Video>();
        }

        public async Task<Course> AddVideoToCourse(long courseId, Video video)
        {
            try
            {
                var course = await _context.Course
                    .Include(c => c.Videos) 
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                {
                    throw new NotFoundException("Course not found.");
                }
                if (course.Videos.Any(v => v.Id == video.Id))
                {
                    throw new InvalidOperationException("Video is already associated with the course.");
                }

                course.Videos.Add(video);

                await _context.SaveChangesAsync();

                return course;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while adding the video to the course.", ex);
            }
        }

        public async Task<List<Course>> GetAllPublicCourses()
        {
            try
            {
                List<Course> courses = await _context.Course
                .Include(c => c.Author)
                .Include(c=>c.Categories)
                .Include(c => c.Ratings )
                .Where(c => c.Status == CourseStatus.PUBLISHED).ToListAsync();
                return courses;
            }
            catch(Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving courses.", ex);
            }
            
        }

        public async Task<List<Course>> SearchCourse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Course>();
            }
            try
            {
                List<Course> courses = await _context.Course
                    .Include(c => c.Author)
                    .Include(c => c.Categories)
                    .Include(c => c.Ratings)
                    .Where(c => c.Status == CourseStatus.PUBLISHED && c.Name.Trim().ToLower().Contains(query.Trim().ToLower())).ToListAsync();
                return courses;
            }catch (Exception ex)
            {
                throw new RepositoryException("Ann error occured while retrieving searched courses.", ex);
            }
        }

        public async Task<List<Course>> FilterCourses(FilterDto filter)
        {
            if (filter == null || filter.Categories == null || (string.IsNullOrWhiteSpace(filter.DifficultyLevel) && filter.Categories.Count == 0))
            {
                return await GetAllPublicCourses();
            }
            else if (string.IsNullOrWhiteSpace(filter.DifficultyLevel) && filter.Categories.Count > 0)
            {
                try
                {
                    List<long> categoryIds = filter.Categories.Select(c => c.Id).ToList();

                    List<Course> courses = await _context.Course
                        .Include(c => c.Author)
                        .Include(c => c.Categories)
                        .Include(c => c.Ratings)
                        .Where(c => c.Status == CourseStatus.PUBLISHED && c.Categories.Any(cat => categoryIds.Contains(cat.Id)))
                        .ToListAsync();

                    return courses;
                }
                catch (Exception ex)
                {
                    throw new RepositoryException("Ann error occured while retrieving searched courses.", ex);
                }
            }
            else if(!string.IsNullOrWhiteSpace(filter.DifficultyLevel) && filter.Categories.Count == 0)
            {
                if (Enum.TryParse(filter.DifficultyLevel, out DifficultyLevel difficultyLevel))
                {
                    List<Course> courses = await _context.Course
                        .Include(c => c.Author)
                        .Include(c => c.Categories)
                        .Include(c => c.Ratings)
                        .Where(c => c.Status == CourseStatus.PUBLISHED && c.DifficultyLevel == difficultyLevel)
                        .ToListAsync();

                    return courses;
                }
                else
                {
                    // Handle the case where parsing the difficulty level failed
                    throw new ArgumentException("Invalid difficulty level provided.");
                }

            }
            else
            {
                List<long> categoryIds = filter.Categories.Select(c => c.Id).ToList();

                if (Enum.TryParse(filter.DifficultyLevel, out DifficultyLevel difficultyLevel))
                {
                    List<Course> courses = await _context.Course
                        .Include(c => c.Author)
                        .Include(c => c.Categories)
                        .Include(c => c.Ratings)
                        .Where(c => c.Status == CourseStatus.PUBLISHED &&
                                    c.Categories.Any(cat => categoryIds.Contains(cat.Id)) &&
                                    c.DifficultyLevel == difficultyLevel)
                        .ToListAsync();

                    return courses;
                }
                else
                {
                    // Handle the case where parsing the difficulty level failed
                    throw new ArgumentException("Invalid difficulty level provided.");
                }
            }
            
        }

        public async Task<Course> UpdateCourseStatusToPublic(long courseId, double price, DifficultyLevel difficultyLevel, List<CategoryDto> categoriesDtos)
        {

            try
            {

                List<long> categoryIds = categoriesDtos.Select(c => c.Id).ToList();

                var categories = await _categoryRepository.GetCategories(categoryIds);

                if(categories.Count < categoriesDtos.Count)
                {
                    throw new NotFoundException($"Some of the passed categories do not exist");
                }

                var course = await _context.Course
                    .Include(c => c.Categories)
                    .Include(c => c.Videos)
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                {
                    throw new NotFoundException($"Course with id {courseId} not found.");
                }

                if(course.Videos.Count < 1)
                {
                    throw new BadDataException("A course that does not have a single video cannot be published");
                }

                course.Status = CourseStatus.PUBLISHED;
                course.Price = price;
                course.DifficultyLevel = difficultyLevel;
                course.Categories = categories;

                await _context.SaveChangesAsync();

                return course;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while updating the course status.", ex);
            }
        }

        public async Task<Course> UpdateNameAndDescription(long userId, long courseId, string name, string description)
        {

            try
            {

                var course = await _context.Course
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync(c => c.Id == courseId && c.Author.Id == userId && c.Status == CourseStatus.DRAFT);

                if (course == null)
                {
                    return null;
                }

                course.Name = name;
                course.Description = description;

                await _context.SaveChangesAsync();

                return course;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while updating the course status.", ex);
            }
        }


    }
}
