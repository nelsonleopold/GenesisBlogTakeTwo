using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using GenesisBlogTakeTwo.Services;
using GenesisBlogTakeTwo.Services.Interfaces;
using GenesisBlogTakeTwo.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Custom Services
builder.Services.AddTransient<DataService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<SlugService>();
builder.Services.AddScoped<IEmailSender, BasicEmailService>();
builder.Services.AddScoped<DisplayService>();
builder.Services.AddScoped<SearchService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Use custom service
var scope = app.Services.CreateScope();
var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
await dataService.SetupDbAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
