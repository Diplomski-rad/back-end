using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Courses_app.Controllers
{

    [ApiController]
    [Route("api/course")]
    public class CourseController : ControllerBase
    {

        private readonly ICourseService _courseService;
        private readonly IPurchaseService _purchaseService;

        public CourseController(ICourseService courseService, IPurchaseService purchaseService)
        {
            _courseService = courseService;
            _purchaseService = purchaseService;

        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPost("create")]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseModel create)
        {
            try
            {
                var res = await _courseService.Add(create);
                return Ok(new { res });


            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetAuthorCourses(long authorId)
        {
            try
            {
                List<CourseDto> res = await _courseService.GetAuthorCourses(authorId);
                if (res != null)
                {
                    return Ok(res);
                }

                return StatusCode(500);

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPost("addvideo")]
        public async Task<IActionResult> AddVideoToCourse([FromForm] AddVideoModel model)
        {
            if (model.file == null || model.file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try {
                CourseDto courseDto = await _courseService.AddVideoToCourse(model);
                return Ok(courseDto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet("videos/{courseId}")]
        public async Task<IActionResult> GetVideos(long courseId)
        {
            try
            {
                List<VideoDto> res = await _courseService.GetCourseVideos(courseId);
                if (res != null)
                {
                    return Ok(res);
                }

                return StatusCode(500);

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedCourses()
        {
            try
            {
                List<CourseDto> res = await _courseService.GetAllPublicCourses();
                if (res != null)
                {
                    return Ok(res);
                }

                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");
            }
        }

        [HttpGet("purchased/{userId}")]
        public async Task<IActionResult> GetPurchasedCourses(long userId)
        {
            try
            {
                List<CourseDto> res = await _purchaseService.GetPurchasedCourses(userId);
                if (res != null)
                {
                    return Ok(res);
                }

                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] string query)
        {
            try 
            {
                return Ok(await _courseService.SearchCourse(query));
            } 
            catch(Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterCourses([FromBody] FilterDto filter)
        {
            if (filter == null)
            {
                return BadRequest("Filter data is required.");
            }
            try
            {
                var courses = await _courseService.FilterCourses(filter);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "UserOnly")]
        [HttpGet("single-purchased/{courseId}")]
        public async Task<IActionResult> GetSinglePurchasedCourse(long courseId)
        {
            try
            {
                var course = await _courseService.GetPurchased(courseId);
                if (course != null)
                {
                    return Ok(course);
                }

                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPut("{courseId}/publish")]
        public async Task<IActionResult> PublishCourse(long courseId, PublishCourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var course = await _courseService.PublishCourse(courseId, request);
                return Ok("Successfully publised");
            }
            
            catch(BadDataException ex)
            {
                return BadRequest(ex.Message);
            }
            
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
