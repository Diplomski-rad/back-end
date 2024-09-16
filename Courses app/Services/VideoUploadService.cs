using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Repository;
using Courses_app.WebSocket;
using Microsoft.AspNetCore.SignalR;

namespace Courses_app.Services
{
    public interface IVideoUploadService
    {
        Task UploadVideoAndAddToCourseAsync(string uploadUrl, TokenResponse tokenResponse, string title, string description, Course course, Author author, byte[] fileBytes);
    }

    public class VideoUploadService : IVideoUploadService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public VideoUploadService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task UploadVideoAndAddToCourseAsync(string uploadUrl,TokenResponse tokenResponse,string title, string description, Course course, Author author, byte[] fileBytes)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var videoService = scope.ServiceProvider.GetRequiredService<IVideoService>();
                var courseRepository = scope.ServiceProvider.GetRequiredService<ICourseRepository>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<CourseHub>>();

                using (var fileStream = new MemoryStream(fileBytes))
                {
                    var uploadResponse = await videoService.UploadFile(fileStream, uploadUrl, tokenResponse.Access_token);

                    string videoId = await videoService.CreateVideo(title, tokenResponse.Access_token, uploadResponse.url, tokenResponse.uid,"description");
                    bool isAdded = await videoService.AddVideoToPlaylist(tokenResponse.Access_token, videoId, course.PlaylistId);

                    var video = new Video(videoId, author, title, description, false);
                    Course updatedCourse = await courseRepository.AddVideoToCourse(course.Id, video);

                    await hubContext.Clients.All.SendAsync("CourseUpdated", updatedCourse.Id);
                }
            }
        }
    }
}
