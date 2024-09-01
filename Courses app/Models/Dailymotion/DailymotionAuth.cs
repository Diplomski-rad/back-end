using Newtonsoft.Json;

namespace Courses_app.Models.Dailymotion
{
    public class DailymotionAuth
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _scope;
        private readonly string _username;
        private readonly string _password;

        public DailymotionAuth(string apiKey, string apiSecret, string scope, string username, string password)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _scope = scope;
            _username = username;
            _password = password;
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

            return tokenResponse;

        }
    }
}
