using Courses_app.Exceptions;
using Courses_app.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses_app.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CoursesAppDbContext _context;

        public CourseRepository(CoursesAppDbContext context)
        {
            _context = context;
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
                    .Include(c => c.Videos)         
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
                List<Course> courses = await _context.Course.Include(c => c.Author).Where(c => c.Status == CourseStatus.PUBLISHED && c.Name.Trim().ToLower().Contains(query.Trim().ToLower())).ToListAsync();
                return courses;
            }catch (Exception ex)
            {
                throw new RepositoryException("Ann error occured while retrieving searched courses.", ex);
            }
        }

        public async Task<Course> UpdateCourseStatusToPublic(long courseId, double price)
        {
            try
            {
                var course = await _context.Course
                    .Include(c => c.Author)
                    .Include(c => c.Videos)
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
