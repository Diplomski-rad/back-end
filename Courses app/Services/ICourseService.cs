using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface ICourseService
    {
        public Task<long> Add(CreateCourseModel createCourse);
        public Task<List<CourseDto>> GetAuthorCourses(long authorId);
        public Task<CourseDto> AddVideoToCourse(AddVideoModel model);
        public Task<List<VideoDto>> GetCourseVideos(long courseId);
        public Task<List<CourseDto>> GetAllPublicCourses();
        public Task<Course> Get(long id);
        public Task<CourseDto> GetPurchased(long courseId);
        public Task<CourseDto> PublishCourse(long courseId, PublishCourseRequest request);
    }
}
