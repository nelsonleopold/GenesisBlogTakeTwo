using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GenesisBlogTakeTwo.Models;

namespace GenesisBlogTakeTwo.Data
{
    public class ApplicationDbContext : IdentityDbContext<BlogUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogPost> BlogPost { get; set; }
        public DbSet<BlogPostComment> BlogPostComment { get; set; }
        public DbSet<Models.Tag> Tag { get; set; }
    }
}