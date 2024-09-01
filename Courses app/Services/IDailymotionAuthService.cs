using Courses_app.Models;

namespace Courses_app.Services
{
    public interface IDailymotionAuthService
    {
        public Task<TokenResponse> GetAccessToken();
    }
}
