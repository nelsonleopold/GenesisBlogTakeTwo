using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GenesisBlogTakeTwo.Models;

namespace GenesisBlogTakeTwo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GenesisBlogTakeTwo.Models.BlogPost> BlogPost { get; set; }
        public DbSet<GenesisBlogTakeTwo.Models.BlogPostComment> BlogPostComment { get; set; }
    }
}