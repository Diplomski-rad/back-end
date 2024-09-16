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

        [Authorize(Policy = "AuthorOnly")]
        [HttpGet("author/{courseId}")]
        public async Task<IActionResult> GetAuthorCourse(long courseId)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var course = await _courseService.GetAuthorCourse(userId, courseId);
                if (course == null)
                {
                    return NotFound("Author don't own this course");
                }
                return Ok(course);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retriving courses. Please try again later.");
            }
        }

        [HttpGet("author/courses/{authorId}")]
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
                string link = await _courseService.AddVideoToCourseAsync(model);
                return Ok(link);
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

        [Authorize(Policy ="UserOnly")]
        [HttpGet("purchased")]
        public async Task<IActionResult> GetPurchasedCourses()
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);


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
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var course = await _purchaseService.GetPurchasedCourse(userId, courseId);
                if (course == null)
                {
                    return Ok(null);
                }
                return Ok(course);

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
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var course = await _courseService.PublishCourse(courseId,userId , request);
                return Ok("Successfully publised");
            }
            catch (NotFoundException ex)
            {
                return Unauthorized("You don't own course with given id.");
            }

            catch (BadDataException ex)
            {
                return BadRequest(ex.Message);
            }
            
            catch(Exception ex)
            {
                throw;
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPut("{courseId}/archive")]
        public async Task<IActionResult> ArchiveCourse(long courseId)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var course = await _courseService.ArchiveCourse(courseId, userId);
                return Ok("Successfully publised");
            }
            
            catch(NotFoundException ex)
            {
                return Unauthorized("You don't own course with given id.");
            }

            catch (BadDataException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPut("{courseId}/update")]
        public async Task<IActionResult> UpdateNameAndDescription(long courseId, UpdateNameAndDescriptionDto request)
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

                var course = await _courseService.UpdateNameAndDescription(userId, courseId, request.Name, request.Description);
                if(course == null)
                {
                    return NotFound("The current author does not own this course");
                }
                else
                {
                    return Ok("Successfully updated");
                }
                
            }

            catch (BadDataException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPut("{courseId}/thumbnail")]
        public async Task<IActionResult> AddThumbnailToCourse([FromForm] IFormFile file, long courseId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Uploaded file is not an image.");

            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var course = await _courseService.AddCourseThumbnail(userId, courseId, file);

                if(course == null)
                {
                    return Unauthorized("You are not owner of this course.");
                }

                return Ok("Thumbnail successfully changed.");


            }catch (BadDataException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured.");
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpPut("{courseId}/video/{videoId}/thumbnail")]
        public async Task<IActionResult> AddThumbnailToVideo([FromForm] IFormFile file, long courseId, string videoId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Uploaded file is not an image.");

            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var video = await _courseService.AddVideoThumbnail(userId, courseId, videoId, file);

                if (video == null)
                {
                    return Unauthorized("You are not owner of this course.");
                }

                return Ok(video);


            }
            catch (BadDataException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured.");
            }
        }
    }
}
