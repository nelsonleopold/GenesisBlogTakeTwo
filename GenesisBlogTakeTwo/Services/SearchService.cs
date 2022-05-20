using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using Microsoft.EntityFrameworkCore;

namespace GenesisBlogTakeTwo.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<BlogPost> Search(string searchString)
        {
            var blogPosts = new List<BlogPost>().AsQueryable();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return blogPosts;
            }

            blogPosts = _context.BlogPost.Include(b => b.BlogPostComments)
                                                .ThenInclude(c => c.Author)
                                             .Include(b => b.Tags)
                                             .Where(b => b.BlogPostState == Enums.BlogPostState.ProductionReady && !b.IsDeleted)
                                             .AsQueryable();


            searchString = searchString.ToLower();

            blogPosts = blogPosts.Where(b => b.Title.ToLower().Contains(searchString) ||
                                             b.Abstract.ToLower().Contains(searchString) ||
                                             b.Content.ToLower().Contains(searchString) ||
                                             b.BlogPostComments.Any(c => c.Comment.ToLower().Contains(searchString) ||
                                                                    c.Author!.FirstName.ToLower().Contains(searchString) ||
                                                                    c.Author!.LastName.ToLower().Contains(searchString)) ||
                                             b.Tags.Any(t => t.Text.ToLower().Contains(searchString)));

            return blogPosts.OrderByDescending(b => b.Created);
        }
    }
}
