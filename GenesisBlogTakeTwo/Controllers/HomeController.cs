using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using MailKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using X.PagedList;

namespace GenesisBlogTakeTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _appSettings;
        private readonly IEmailSender _emailService;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              IConfiguration appSettings,
                              IEmailSender emailService)
        {
            _logger = logger;
            _context = context;
            _appSettings = appSettings;
            _emailService = emailService;
        }

        //public IActionResult Index()
        //{
        //    var blogPosts = _context.BlogPost.ToList();
        //    return View(blogPosts);
        //}

        public async Task<IActionResult> Index(int? pageNum)
        {
            pageNum ??= 1;
            var pageSize = 3;

            var posts = await _context.BlogPost.Where(b => b.BlogPostState == Enums.BlogPostState.ProductionReady && !b.IsDeleted)
                                               .OrderByDescending(b => b.Created)
                                               .ToPagedListAsync(pageNum, pageSize);

            return View(posts);
        }

        public IActionResult ContactMe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(string name, string email, string phone, string subject, string message)
        {
            var myEmail = _appSettings["SmtpSettings:UserName"];

            // Incorporate as much info into the body of the email
            StringBuilder _builder = new(message);
            _builder.AppendLine("<br><br>");
            _builder.AppendLine($"Sender's Information: <br>");
            _builder.AppendLine($"Sender's Name: {name} <br>");
            _builder.AppendLine($"Sender's Email: {email} <br>");
            _builder.AppendLine($"Sender's Phone: {phone} <br>");


            await _emailService.SendEmailAsync(myEmail, subject, _builder.ToString());
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}