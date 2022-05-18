using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    }
}
