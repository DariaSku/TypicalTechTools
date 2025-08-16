using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Authorization;


namespace TypicalTechTools.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Privacy()
        {
            // Load the file’s bytes into memory
            var imageBytes = System.IO.File.ReadAllBytes(@"C:\images\Privacy.jpg");

            // Convert the byte[] to a Base64 string
            var imageBytesString = Convert.ToBase64String(imageBytes);

            // Send the string into the View
            ViewBag.ImageData = imageBytesString;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}