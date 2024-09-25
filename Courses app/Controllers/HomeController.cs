using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Courses_app.Controllers
{
    public class HomeController : Controller
    {



        public HomeController()
        {

        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}