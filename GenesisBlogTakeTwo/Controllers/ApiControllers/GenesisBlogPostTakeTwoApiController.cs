using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Enums;
using GenesisBlogTakeTwo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenesisBlogTakeTwo.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenesisBlogPostTakeTwoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenesisBlogPostTakeTwoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/GetTopXPosts/{num}")]
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

        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            if (_context.BlogPost == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPost.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            if (_context.BlogPost == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPost' is null.");
            }

            _context.BlogPost.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            if (_context.BlogPost == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPost.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPost.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPost?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
