using Courses_app.Dto;

namespace Courses_app.Services
{
    public interface IVideoService
    {
        public Task<string> UploadVideo(Stream videoStream, string title, string playlistId);
        public Task<string> CreatePlaylist(string name);
        public Task<string> UpdatePlaylist(string playlistId, string name);
        public Task<string> GetAccessToken();
        public Task<string> GetPlayerForVideo(string videoId);

        public Task<UploadUrlResponse> UploadVideoTest(Stream videoStream, string title, string playlistId);

        public Task<UploadUrlResponse> GetUploadUrl(string accessToken);
        public Task<UploadResponse> UploadFile(Stream videoStream, string uploadUrl, string accessToken);
        public Task<string> CreateVideo(string title, string accessToken, string uploadUrl, string uid, string description);
        public Task<bool> AddVideoToPlaylist(string accessToken, string videoId, string playslstId);
        public Task<EncodingProgressResponse> CheckVideoEncodingProgress(string videoId);


    }
}
