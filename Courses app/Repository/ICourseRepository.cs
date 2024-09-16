using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface ICourseRepository
    {
        public Task<long> Add(Course course);
        public Task<Course> Get(long id);
        public Task<Course> GetAuthorCourse(long authorId, long courseId);
        public Task<List<Course>> GetCoursesByIds(List<long> ids);
        public Task<List<Course>> GetAuthorCourses(long authorId);
        public Task<Course> AddVideoToCourse(long courseId, Video video);
        public Task<List<Video>> GetCourseVideos(long courseId);
        public Task<List<Course>> GetAllPublicCourses();
        public Task<List<Course>> SearchCourse(string query);
        public Task<List<Course>> FilterCourses(FilterDto filter);
        public Task<Course> UpdateCourseStatusToPublic(long courseId, long userId, double price, DifficultyLevel difficultyLevel, List<CategoryDto> categories);
        public Task<Course> ArchiveCourse(long courseId, long userId);
        public Task<Course> UpdateNameAndDescription(long userId, long courseId, string name, string description);
        public Task<Course> AddCourseThumbnail(long authorId, long courseId, string thumbnail);
        public Task<Video> AddVideoThumbnail(long authorId, long courseId, string videoId, string thumbnail);
        public Task<Video> UpdateVideoPublishedStatus(string videoId);

    }
}
