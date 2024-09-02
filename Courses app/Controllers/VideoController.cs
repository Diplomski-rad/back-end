using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
                var videoId = await _videoService.UploadVideo(video.OpenReadStream(), "New playlist video", "x8no2q");

                return Ok(new { VideoId = videoId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("{videoId}/player")]
        public async Task<IActionResult> GetPlayer(string videoId)
        {
            try
            {
                var url = await _videoService.GetPlayerForVideo(videoId);

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


    }
}
