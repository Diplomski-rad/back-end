using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Services;
using Courses_app.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Courses_app.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ICourseService _courseService;
        private readonly ILogger<VideoController> _logger;
        private readonly IHubContext<CourseHub> _hubContext;

        public VideoController(IVideoService videoService, ILogger<VideoController> logger, ICourseService courseService, IHubContext<CourseHub> hubContext)
        {
            _videoService = videoService;
            _logger = logger;
            _courseService = courseService;
            _hubContext = hubContext;
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
                var response = await _videoService.UploadVideoTest(video.OpenReadStream(), "New playlist video", "x8ouyg");

                return Ok(response);
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

        [Authorize(Policy = "AuthorOnly")]
        [HttpGet("{videoId}/encoding-progress")]
        public async Task<IActionResult> GetEncodingProgress(string videoId)
        {
            try
            {
                var response = await _videoService.CheckVideoEncodingProgress(videoId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ProccessWebhookEvent(DailymotionWebhookEvent webhookEvent){
            try
            {
                if(webhookEvent.Type == "video.published")
                {
                    await _courseService.UpdateVideoPublishedStatus(webhookEvent.Data.Video_id);
                    //await _hubContext.Clients.All.SendAsync("VideoPublished", webhookEvent.Data.Video_id);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch
            {
                return StatusCode(500);
            }
        }


    }
}
