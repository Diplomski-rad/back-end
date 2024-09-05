using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface IRatingRepository
    {
        public Task<long> Add(Rating rating);
        //public Task<CourseRatingsDto> GetRatingsForCourse(long courseId);
        public Task<Rating> GetUsersRatingForCourse(long userId, long courseId);
    }
}
