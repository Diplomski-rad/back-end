using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface ICourseService
    {
        public Task<long> Add(CreateCourseModel createCourse);
        public Task<CourseDto> GetAuthorCourse(long authorId, long courseId);
        public Task<List<CourseDto>> GetAuthorCourses(long authorId);
        public Task<CourseDto> AddVideoToCourse(AddVideoModel model);
        public Task<List<VideoDto>> GetCourseVideos(long courseId);
        public Task<List<CourseDto>> GetAllPublicCourses();
        public Task<List<CourseDto>> SearchCourse(string query);
        public Task<List<CourseDto>> FilterCourses(FilterDto filter);
        public Task<Course> Get(long id);
        public Task<List<Course>> GetCoursesByIds(List<long> ids);
        public Task<CourseDto> GetPurchased(long courseId);
        public Task<CourseDto> PublishCourse(long courseId, PublishCourseRequest request);
        public Task<Course> UpdateNameAndDescription(long userId, long courseId, string name, string description);
        public Task<Course> AddCourseThumbnail(long authorId, long courseId, IFormFile thumbnail);
        public Task<VideoDto> AddVideoThumbnail(long authorId, long courseId, string videoId, IFormFile image);

        public Task<string> AddVideoToCourseAsync(AddVideoModel model);
        public Task<Video> UpdateVideoPublishedStatus(string videoId);
    }
}
