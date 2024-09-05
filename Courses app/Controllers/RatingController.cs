using Courses_app.Dto;
using Courses_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace Courses_app.Controllers
{
    [ApiController]
    [Route("api/rating")]
    public class RatingController : ControllerBase
    {

        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RatingDto rating)
        {
            try
            {
                await _ratingService.Add(rating);
                return Ok();

            }catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        //[HttpGet("course/{courseId}")]
        //public async Task<IActionResult> GetCourseRatings(long courseId)
        //{
        //    try
        //    {
        //        var courseRatings = await _ratingService.GetCourseRatings(courseId);
        //        return Ok(courseRatings);

        //    }catch (Exception ex)
        //    {
        //        throw new Exception($"{ex.Message}", ex);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] long userId, long courseId)
        {
            try
            {
                var rating = await _ratingService.GetUsersRatingForCourse(userId, courseId);
                if(rating == null)
                {
                    return Ok(null);
                }

                return Ok(rating);

            }catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
