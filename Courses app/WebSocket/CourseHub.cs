using Microsoft.AspNetCore.SignalR;

namespace Courses_app.WebSocket
{
    public class CourseHub : Hub
    {
        public async Task NotifyCourseUpdated(long updatedCourseId)
        {
            await Clients.All.SendAsync("CourseUpdated", updatedCourseId);
        }

        public async Task NotifyVideoPublished(long videoId)
        {
            await Clients.All.SendAsync("VideoPublished", videoId);
        }
    }
}
