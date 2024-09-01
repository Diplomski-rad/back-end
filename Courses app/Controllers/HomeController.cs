using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Courses_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDailymotionAuthService _authService;
        private readonly IVideoService _videoService;


        public HomeController(ILogger<HomeController> logger, IDailymotionAuthService authService, IVideoService videoService, IConfiguration configuration)
        {
            _logger = logger;
            _authService = authService;
            _videoService = videoService;
        }

        public async Task<ActionResult> Index()
        {
            //DailymotionAuth dailymotionAuth = new DailymotionAuth("c3ddc2ea3e1db5f4ed62", "aec8b37e76e24921aeffd8ac24a22d10fd6e65c0", "manage_videos", "coursesapp.2024@gmail.com", "Coursesapp.29072001");
            string filePath = @"C:\Users\lukar\Videos\2024-08-04 13-36-54.mkv";
            TokenResponse response = await _authService.GetAccessToken();

            //var res = _authService.GetAccessToken();
            //var res = dailymotionAuth.GetAccessToken();
            //DailymotionVideoUploader uploader = new DailymotionVideoUploader(dailymotionAuth);
            //var res = _videoService.UploadVideo(filePath,"This is title");
            //var response = uploader.UploadVideo(filePath, "Exapmle private video");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}