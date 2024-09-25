using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using static System.Formats.Asn1.AsnWriter;
using Courses_app.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Courses_app.Services
{
    public class DailymotionAuthService : IDailymotionAuthService
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _scope;
        private readonly string _username;
        private readonly string _password;

        public DailymotionAuthService()
        {
            _apiKey = "c3ddc2ea3e1db5f4ed62";
            _apiSecret = "aec8b37e76e24921aeffd8ac24a22d10fd6e65c0";
            _scope = "manage_videos";
            _username = "coursesapp.2024@gmail.com";
            _password = "Coursesapp.29072001";
        }

        public async Task<TokenResponse> GetAccessToken()
        {
            var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type", "password"),
                new KeyValuePair<string,string>("client_id", _apiKey),
                new KeyValuePair<string,string>("client_secret", _apiSecret),
                new KeyValuePair<string,string>("scope", _scope),
                new KeyValuePair<string,string>("username", _username),
                new KeyValuePair<string,string>("password", _password)
            });

            var response = await client.PostAsync("https://api.dailymotion.com/oauth/token", content);
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_token);

            var webhook_content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("webhook_url","https://pleased-romantic-sawfly.ngrok-free.app/api/videos/webhook"),
                new KeyValuePair<string, string>("webhook_events", "video.published"),
            });

            await client.PostAsync("https://api.dailymotion.com/user/me", webhook_content);

            return tokenResponse;
        }
    }
}
