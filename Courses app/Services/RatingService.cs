using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Repository;

namespace Courses_app.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<long> Add(RatingDto ratingDto)
        {
            return await _ratingRepository.Add(new Rating(ratingDto));
        }

        //public async Task<CourseRatingsDto> GetCourseRatings(long courseId)
        //{
        //    return await _ratingRepository.GetRatingsForCourse(courseId);
        //}

        public async Task<RatingDto> GetUsersRatingForCourse(long userId, long courseId)
        {
            Rating rating =  await _ratingRepository.GetUsersRatingForCourse(userId, courseId);

            if(rating == null)
            {
                return null;
            }

            return new RatingDto(rating);
        }


    }
}
