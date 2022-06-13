using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Enums;
using GenesisBlogTakeTwo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenesisBlogTakeTwo.Controllers.ApiControllers
{
    /// <summary>
    /// This controller intercepts requests for BlogPost data over Http
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GenesisBlogPostTakeTwoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenesisBlogPostTakeTwoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ActionName("GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTopXPosts(int num)
        {
            if (_context.BlogPost == null)
            {
                return NotFound();
            }

            var blogPosts = _context.BlogPost.Where(b => b.BlogPostState == BlogPostState.ProductionReady && !b.IsDeleted)
                                             .OrderByDescending(b => b.Created)
                                             .Take(num);

            return await blogPosts.ToListAsync();
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPost?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
