using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy ="UserOnly")]
        public async Task<IActionResult> Add([FromBody] AddRatingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                

                await _ratingService.Add(new RatingDto { UserId = userId, CourseId = request.CourseId, RatingValue = request.RatingValue});
                return Ok();

            }catch(BadDataException ex)
            {
                return BadRequest("The given user or course does not exist");
            }
            
            catch (Exception ex)
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
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> Get([FromQuery] long courseId)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

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
