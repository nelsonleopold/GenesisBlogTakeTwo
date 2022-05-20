using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GenesisBlogTakeTwo.Services
{
    // This is a wrapper class that will run whenever the application is started
    // It will check the database AspUserRoles table and look to see if any Roles
    // exist. (could maybe be rewritten to check for specific roles?)
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext context,
                           RoleManager<IdentityRole> roleManager,
                           UserManager<BlogUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // A method used to set up roles during startup
        public async Task SetupDbAsync()
        {
            // Run a single commangd to execute all of the migrations
            await _context.Database.MigrateAsync();

            // Call a private method that adds sample Roles, Users, and Tags
            // into the system 
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedTagsAsync();
            await SeedTestBlogs();

        }

        // A private method that seeds the Roles for Admin and Moderator on
        // application start.
        private async Task SeedRolesAsync()
        {
            // This will run every time the application is started so make sure
            // nothing is done if there are ANY records in the Roles table
            if (_roleManager.Roles.Count() == 0)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Moderator"));
            }
        }

        // A private method that seeds Users on application start. 
        private async Task SeedUsersAsync()
        {
            // Create a new instance of BlogUser
            string adminEmail = "nelson.thedev@mailinator.com";
            BlogUser adminBlogUser = new()
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "nelson",
                LastName = "thedev",
                PhoneNumber = "555.555.5555",
                EmailConfirmed = true,
            };

            string modEmail = "pparker@mailinator.com";
            BlogUser modBlogUser = new()
            {
                UserName = modEmail,
                Email = modEmail,
                FirstName = "Peter",
                LastName = "Parker",
                PhoneNumber = "555.555.1212",
                EmailConfirmed = true,
            };

            var newAdminUser = await _userManager.FindByEmailAsync(adminEmail);
            var newModUser = await _userManager.FindByEmailAsync(modEmail);

            if(newAdminUser is null)
            {
                await _userManager.CreateAsync(adminBlogUser, "Abc&123!");
                await _userManager.AddToRoleAsync(adminBlogUser, "Admin");
            }
            if(newModUser is null)
            {
                await _userManager.CreateAsync(modBlogUser, "Abc&123!");
                await _userManager.AddToRoleAsync(modBlogUser, "Moderator");
            }
        }

        private async Task SeedTagsAsync()
        {
            if (_context.Tag.Count() == 0)
            {
                await _context.AddAsync(new Tag() { Text = "Role Based Security" });
                await _context.AddAsync(new Tag() { Text = "Scaffolding Against A Model" });
                await _context.AddAsync(new Tag() { Text = "Customizing Identity" });
                await _context.AddAsync(new Tag() { Text = "Working With Interfaces" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedTestBlogs()
        {
            // this checks to see if there are any blogposts before seeding programmatic ones
            if (_context.BlogPost.Any())
                return;

            // generate lorem ispum text for testing
            StringBuilder body = new("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Arcu vitae elementum curabitur vitae nunc sed velit dignissim.Consequat id porta nibh venenatis cras. Magna sit amet purus gravida quis blandit turpis cursus in. Proin nibh nisl condimentum id.Porta lorem mollis aliquam ut porttitor. Praesent elementum facilisis leo vel fringilla est ullamcorper eget nulla. Purus gravida quis blandit turpis cursus in hac.Imperdiet massa tincidunt nunc pulvinar sapien et ligula. Tellus mauris a diam maecenas sed enim ut sem.Nunc eget lorem dolor sed viverra. Tellus in hac habitasse platea dictumst vestibulum rhoncus. Mauris sit amet massa vitae tortor condimentum lacinia quis vel.");
            body.AppendLine("Turpis egestas maecenas pharetra convallis.Sed velit dignissim sodales ut eu sem.Ultricies integer quis auctor elit sed vulputate.Elementum integer enim neque volutpat ac. Risus pretium quam vulputate dignissim suspendisse in. Amet consectetur adipiscing elit ut aliquam purus sit amet.At ultrices mi tempus imperdiet.Vel facilisis volutpat est velit egestas dui id ornare.Orci dapibus ultrices in iaculis.Sed vulputate odio ut enim blandit volutpat maecenas volutpat.Quam nulla porttitor massa id neque aliquam vestibulum morbi.Pretium quam vulputate dignissim suspendisse in.");
            body.AppendLine("Purus semper eget duis at tellus at.Malesuada fames ac turpis egestas integer eget aliquet nibh.Purus non enim praesent elementum facilisis leo vel fringilla.Tortor id aliquet lectus proin nibh nisl condimentum id venenatis. Hac habitasse platea dictumst quisque sagittis purus.Dui vivamus arcu felis bibendum ut tristique et egestas.Cras semper auctor neque vitae tempus quam pellentesque nec.Convallis convallis tellus id interdum velit. Diam maecenas sed enim ut.Adipiscing commodo elit at imperdiet dui accumsan.Netus et malesuada fames ac.Habitasse platea dictumst quisque sagittis purus sit amet. Enim praesent elementum facilisis leo vel fringilla est ullamcorper eget. Id diam maecenas ultricies mi eget mauris.Lorem dolor sed viverra ipsum nunc aliquet bibendum.");
            body.AppendLine("Tristique magna sit amet purus gravida quis blandit turpis.Morbi tempus iaculis urna id volutpat. Vulputate odio ut enim blandit volutpat maecenas volutpat blandit aliquam. Ultricies tristique nulla aliquet enim.Quam pellentesque nec nam aliquam sem et tortor consequat id. Est ullamcorper eget nulla facilisi etiam dignissim diam. Purus ut faucibus pulvinar elementum integer enim neque. Pellentesque elit ullamcorper dignissim cras tincidunt lobortis feugiat vivamus at. Tempor orci eu lobortis elementum nibh tellus molestie. Consequat nisl vel pretium lectus quam id leo in vitae.Purus faucibus ornare suspendisse sed.Iaculis urna id volutpat lacus.Mattis vulputate enim nulla aliquet porttitor lacus luctus. Proin nibh nisl condimentum id venenatis a condimentum. Etiam non quam lacus suspendisse faucibus interdum.Et tortor at risus viverra adipiscing at.Fermentum leo vel orci porta non pulvinar.Tincidunt nunc pulvinar sapien et ligula ullamcorper malesuada proin libero. Nibh praesent tristique magna sit amet. Tellus elementum sagittis vitae et leo duis.");
            body.AppendLine("Vulputate enim nulla aliquet porttitor lacus luctus accumsan tortor posuere. Orci eu lobortis elementum nibh tellus molestie nunc non blandit. Odio pellentesque diam volutpat commodo sed egestas egestas fringilla phasellus. Mauris vitae ultricies leo integer malesuada nunc vel. Ultrices in iaculis nunc sed.Est ullamcorper eget nulla facilisi etiam dignissim diam. Facilisis gravida neque convallis a cras semper auctor. Porttitor rhoncus dolor purus non enim. Id nibh tortor id aliquet lectus. Consequat semper viverra nam libero justo laoreet.At augue eget arcu dictum varius duis at. Lectus mauris ultrices eros in cursus turpis. Sed risus pretium quam vulputate dignissim suspendisse in est ante. Cursus turpis massa tincidunt dui.Ut tellus elementum sagittis vitae et leo duis.");


            for (var loop = 1; loop <= 10; loop++)
            {
                _context.Add(new BlogPost()
                {
                    Title = $"Blog Title {loop}: The importance of {loop}'s in coding...",
                    Abstract = $"Have you ever wondered why the number {loop} plays such a significant role in coding?",
                    Content = body.ToString(),
                    Created = DateTime.UtcNow.AddDays(-loop),
                    Slug = $"blog-title-{loop}-the-importance-of-{loop}s-in-coding"
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
