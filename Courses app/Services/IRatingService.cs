using Courses_app.Dto;

namespace Courses_app.Services
{
    public interface IRatingService
    {
        public Task<long> Add(RatingDto ratingDto);
        //public Task<CourseRatingsDto> GetCourseRatings(long courseId);
        public Task<RatingDto> GetUsersRatingForCourse(long userId, long courseId);
    }
}
