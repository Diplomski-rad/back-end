using Courses_app.Dto;
using Courses_app.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace Courses_app.Services
{
    public class VideoService : IVideoService
    {

        private readonly IDailymotionAuthService _auth;

        public VideoService(IDailymotionAuthService authService)
        {

            _auth = authService;

        }

        public async Task<string> GetAccessToken()
        {
            try {
                TokenResponse tokenResponse = await _auth.GetAccessToken();
                return tokenResponse.Access_token;
            }
            catch (Exception ex) {
                throw;
            }
        }
        public async Task<string> UploadVideo(Stream videoStream, string title, string playlistId)
        {
            TokenResponse tokenResponse = await _auth.GetAccessToken();

            // Get upload URL
            var uploadUrlResponse = await GetUploadUrl(tokenResponse.Access_token);
            var uploadUrl = uploadUrlResponse.upload_url;

            string description = "This is description";
            // Upload video
            var uploadResponse = await UploadFile(videoStream, uploadUrl, tokenResponse.Access_token);

            string videoId = await CreateVideo(title, tokenResponse.Access_token, uploadResponse.url, tokenResponse.uid, description);

            bool isAdded = await AddVideoToPlaylist(tokenResponse.Access_token, videoId, playlistId);

            return videoId;
        }

        private async Task<UploadUrlResponse> GetUploadUrl(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var response = await client.GetAsync("https://api.dailymotion.com/file/upload");

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UploadUrlResponse>(json);
        }

        private async Task<UploadResponse> UploadFile(Stream videoStream, string uploadUrl, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(videoStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("video/*"); // Adjust content type based on your video format
            content.Add(fileContent, "file", "video.mkv");

            var response = await client.PostAsync(uploadUrl, content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UploadResponse>(json);
        }

        private async Task<string> CreateVideo(string title, string accessToken, string uploadUrl, string uid, string description)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("url",uploadUrl),
                new KeyValuePair<string, string>("title", title),
                new KeyValuePair<string, string>("is_created_for_kids", "false"),
                new KeyValuePair<string, string>("published", "true"),
                new KeyValuePair<string, string>("private", "true"),
                //new KeyValuePair<string, string>("password", "password123"),
                new KeyValuePair<string, string>("channel", "school"),
                new KeyValuePair<string, string>("description", description),
            });

            try
            {
                var response = await client.PostAsync($"https://api.dailymotion.com/user/{uid}/videos", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var videoResponse = JsonConvert.DeserializeObject<VideoResponse>(json); // Define VideoResponse class based on API response
                    return videoResponse.id;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create video: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> AddVideoToPlaylist(string accessToken, string videoId, string playslstId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var response = await client.PostAsync($"https://api.dailymotion.com/playlist/{playslstId}/videos/{videoId}", null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to add to playlsit: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> CreatePlaylist(string name)
        {
            TokenResponse tokenResponse = await _auth.GetAccessToken();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_token);

            try
            {
                var response = await client.PostAsync($"https://api.dailymotion.com/me/playlists?name={name}&private=true", null);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var playlistResponse = JsonConvert.DeserializeObject<PlaylistReesponse>(json); // Define VideoResponse class based on API response
                    return playlistResponse.id;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create playlist: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UpdatePlaylist(string playlistId, string name)
        {
            TokenResponse tokenResponse = await _auth.GetAccessToken();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_token);

            try
            {

                var response = await client.PostAsync($"https://api.dailymotion.com/playlist/{playlistId}?name={name}", null);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var playlistResponse = JsonConvert.DeserializeObject<PlaylistReesponse>(json); // Define VideoResponse class based on API response
                    return playlistResponse.id;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create playlist: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<string> GetPlayerForVideo(string videoId)
        {
            TokenResponse tokenResponse = await _auth.GetAccessToken();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_token);

            try
            {
                var response = await client.GetAsync($"https://api.dailymotion.com/video/{videoId}?fields=private_id");
                var res = await client.PostAsync($"https://api.dailymotion.com/me/players?label=Player&enable_startscreen_dm_link=false&enable_sharing_url_location=false&enable_sharing=false&enable_paid_partnership_label=false&enable_info=false&enable_dm_logo=false&enable_custom_recommendations=false&enable_channel_link=false&enable_automatic_recommendations=false", null);
                if (response.IsSuccessStatusCode && res.IsSuccessStatusCode)
                {
                    var jsonPrivateId = await response.Content.ReadAsStringAsync();
                    var privateIdResponse = JsonConvert.DeserializeObject<PrivateIdResponse>(jsonPrivateId);
                    var jsonPlayer = await res.Content.ReadAsStringAsync();
                    var playerResponse = JsonConvert.DeserializeObject<CreatePlayerResponse>(jsonPlayer);
                    if(playerResponse != null && privateIdResponse != null)
                    {
                        string url = $"{playerResponse.Embed_html_url}?video={privateIdResponse.Private_Id}&mute=false";
                        return url;
                    }
                    else
                    {
                        throw new Exception("Failed to create player");
                    }
                    
                }
                else
                {
                    throw new Exception("Failed to create player");
                }

            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }


    public class UploadUrlResponse
    {
        public string upload_url { get; set; }
        public string progress_url { get; set; }
    }

    public class UploadResponse
    {
        public string url { get; set; }
    }

    public class VideoResponse
    {
        public string id { get; set; }
        public string title { get; set; }

        public string channel { get; set; }
        public string owner { get; set; }
    }

    public class PlaylistReesponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
    }
}
