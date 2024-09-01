using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface ICourseService
    {
        public Task<Course> Add(CreateCourseModel createCourse);
        public Task<List<Course>> GetAuthorCourses(long authorId);
        public Task<Course> AddVideoToCourse(AddVideoModel model);
        public Task<List<Video>> GetCourseVideos(long courseId);
        public Task<List<Course>> GetAllPublicCourses();
        public Task<Course> Get(long id);
        public Task<PurchasedCourse> GetPurchased(long courseId);
        public Task<Course> PublishCourse(long courseId, PublishCourseRequest request);
    }
}
