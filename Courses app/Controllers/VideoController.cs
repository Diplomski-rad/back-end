using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace Courses_app.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;

        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile video)
        {
            if (video == null || video.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Call the UploadVideo method from the service, passing the stream directly
                var videoId = await _videoService.UploadVideo(video.OpenReadStream(), "New playlist video", "x8no2q");

                return Ok(new { VideoId = videoId });
            }
            catch (Exception ex)
            {
                // Handle errors accordingly
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }

    }
}
