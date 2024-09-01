namespace Courses_app.Services
{
    public interface IVideoService
    {
        public Task<string> UploadVideo(Stream videoStream, string title, string playlistId);
        public Task<string> CreatePlaylist(string name);
        
    }
}
