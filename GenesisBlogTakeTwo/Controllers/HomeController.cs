using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GenesisBlogTakeTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, 
                              ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var blogPosts = _context.BlogPost.ToList();
            return View(blogPosts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}