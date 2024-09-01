using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface ICourseRepository
    {
        public Task<Course> Add(Course course);
        public Task<Course> Get(long id);
        public Task<List<Course>> GetAuthorCourses(long authorId);
        public Task<Course> AddVideoToCourse(long courseId, Video video);
        public Task<List<Video>> GetCourseVideos(long courseId);
        public Task<List<Course>> GetAllPublicCourses();
        public Task<Course> UpdateCourseStatusToPublic(long courseId, double price);
    }
}
