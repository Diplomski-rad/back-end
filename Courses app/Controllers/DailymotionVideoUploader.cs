using Courses_app.Models;
using Courses_app.Models.Dailymotion;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Courses_app.Controllers
{
    public class DailymotionVideoUploader
    {
        private readonly DailymotionAuth _auth;
        public DailymotionVideoUploader(DailymotionAuth auth) 
        {
            _auth = auth;   
        }

        public async Task UploadVideo(string filePath, string title)
        {
            TokenResponse tokenResponse = await _auth.GetAccessToken();

            // Get upload URL
            var uploadUrlResponse = await GetUploadUrl(tokenResponse.Access_token);
            var uploadUrl = uploadUrlResponse.upload_url;

            string description = "This is description";
            // Upload video
            var uploadResponse = await UploadFile(filePath, uploadUrl, tokenResponse.Access_token);
            var videoId = await CreateVideo(title, tokenResponse.Access_token, uploadResponse.url, tokenResponse.uid, description);
            //var videoId = uploadResponse.id;

            //await GetVideoInfo(videoId, tokenResponse.Access_token);

            // Publish video
            //await PublishVideo(videoId, title, description, tokenResponse.Access_token);
        }

        private async Task<UploadUrlResponse> GetUploadUrl(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var response = await client.GetAsync("https://api.dailymotion.com/file/upload");

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UploadUrlResponse>(json);
        }

        private async Task<UploadResponse> UploadFile(string filePath, string uploadUrl, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(File.OpenRead(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("video/*"); // Adjust content type based on your video format
            content.Add(fileContent, "file", Path.GetFileName(filePath));

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
                new KeyValuePair<string, string>("playlist_id", "x8no2q")
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

        //private async Task GetVideoInfo(string videoId, string accessToken)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //    var response = await client.GetAsync($"https://api.dailymotion.com/video/{videoId}?fields=id,url,published");

        //}

        //private async Task PublishVideo(string videoId, string title, string description, string accessToken)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


        //    var content = new FormUrlEncodedContent(new[]
        //    {
        //        new KeyValuePair<string, string>("title", title),
        //        new KeyValuePair<string, string>("is_created_for_kids", "false"),
        //        new KeyValuePair<string, string>("published", "true"),
        //        new KeyValuePair<string, string>("channel", "school"),
        //        new KeyValuePair<string, string>("description", description),
        //    });

        //    await client.PutAsync($"https://api.dailymotion.com/video/{videoId}", content);
        //}
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

    public class GetVideoInfoResponse
    {
        public string id { get; set; }
        public string url { get; set; }
        public string published { get; set; }
    }
}


