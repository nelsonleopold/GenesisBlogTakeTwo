#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GenesisBlogTakeTwo.Data;
using GenesisBlogTakeTwo.Models;
using GenesisBlogTakeTwo.Enums;
using GenesisBlogTakeTwo.Services.Interfaces;
using GenesisBlogTakeTwo.Utilities;
using Microsoft.AspNetCore.Authorization;
using GenesisBlogTakeTwo.Services;

namespace GenesisBlogTakeTwo.Controllers
{
    [Authorize(Roles ="Admin,Moderator")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;
        private readonly SearchService _searchService;

        public BlogPostsController(ApplicationDbContext context,
                                   IImageService imageService,
                                   SlugService slugService,
                                   SearchService searchService)
        {
            _context = context;
            _imageService = imageService;
            _slugService = slugService;
            _searchService = searchService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            var blogPost = await _context.BlogPost.Include(b => b.Tags).ToListAsync();
            return View(blogPost);
        }

        // GET: BlogPosts in development state
        public async Task<IActionResult> InDevIndex()
        {
            var blogPost = await _context.BlogPost.Include(b => b.Tags)
                                                  .Where(b => b.BlogPostState == BlogPostState.InDevelopment)
                                                  .ToListAsync();
            return View("Index", blogPost);
        }

        // GET: BlogPosts in preview state
        public async Task<IActionResult> InPreviewIndex()
        {
            var blogPost = await _context.BlogPost.Include(b => b.Tags)
                                                  .Where(b => b.BlogPostState == BlogPostState.InPreview)
                                                  .ToListAsync();
            return View("Index", blogPost);
        }

        // POST: Search function this should work for anonymous users
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchPosts(string searchString)
        {

            var searchData = _searchService.Search(searchString);
            return View(searchData);
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPost.Include(b => b.Tags)
                                                  .Include(b => b.BlogPostComments)
                                                    .ThenInclude(c => c.Author)
                                                  .FirstOrDefaultAsync(m => m.Slug == slug);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text");
            ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
            //TempData["ImageFile"] = blogPost.ImageFile;
            //TempData["ImageData"] = blogPost.ImageData;
            //TempData["ImageType"] = blogPost.ImageType;
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Abstract,Content,IsDeleted,BlogPostState,ImageFile")] BlogPost blogPost, List<int> tagIds)
        {
            if (ModelState.IsValid)
            {
                var slug = _slugService.URLFriendly(blogPost.Title);
                if (_context.BlogPost.Any(b => b.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The Title must be unique.");
                    ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);
                    ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
                    return View(blogPost);
                }
                else
                {
                    blogPost.Slug = slug;
                }

                // verify that IFormFile isn't null before interacting with it
                if (blogPost.ImageFile is not null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }

                // Associate any/all tags with the BlogPost
                foreach (var tagId in tagIds)
                {
                    // do something...
                    blogPost.Tags.Add(await _context.Tag.FindAsync(tagId));
                }

                // Created date/time is generated programmatically
                blogPost.Created = DateTime.SpecifyKind(blogPost.Created, DateTimeKind.Utc);

                // when using quill rich text editor (or tinyMCE editor), content will never be an empty string because
                // quill auto inserts <p><br></p>; it will replace the <br> tag with whatever is 
                // entered by the user; so this is a check for no data being entered by user. Should
                // refactor this into a _configuration setting?
                if (blogPost.Content == "<p><br></p>")
                {
                    ModelState.AddModelError("Content", "The Content field is required.");
                    ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);
                    ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
                    TempData["ImageFile"] = blogPost.ImageFile;
                    TempData["ImageData"] = blogPost.ImageData;
                    TempData["ImageType"] = blogPost.ImageType;
                    return View();
                }

                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // data should persist even if ModelState is invalid
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPost.Include(b => b.Tags)
                                                  .FirstOrDefaultAsync(b => b.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            //4th parameter in a multiSelect list is a List<int> representing the current selections
            var selectedTags = blogPost.Tags.Select(b => b.Id).ToList();

            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", selectedTags);
            ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,IsDeleted,BlogPostState,ImageFile")] BlogPost blogPost, IFormFile theImage, List<int> tagIds)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Slug");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.BlogPost.Include(b => b.Tags)
                                                              .FirstOrDefaultAsync(b => b.Id == blogPost.Id);

                    foreach (var tag in existingPost.Tags)
                    {
                        existingPost.Tags.Remove(tag);
                    }
                    await _context.SaveChangesAsync();

                    existingPost.Updated = blogPost.Updated;
                    existingPost.Title = blogPost.Title;
                    existingPost.Abstract = blogPost.Abstract;
                    existingPost.Content = blogPost.Content;
                    existingPost.BlogPostState = blogPost.BlogPostState;

                    existingPost.Updated = DateTime.UtcNow;

                    // get a list of all slugs
                    List<string> slugs = await _context.BlogPost.Select(b => b.Slug).ToListAsync();
                    // check if slug is unique

                    // loop over List<string> slugs and check for uniqueness
                    foreach (string slug in slugs)
                    {
                        if (blogPost.Slug != slug)
                        {
                            existingPost.Slug = _slugService.URLFriendly(blogPost.Title);
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "Sorry, you must enter something for title");
                            return RedirectToAction(nameof(Create));
                        }
                    }

                    ////TODO: Image Service
                    if (theImage is not null)
                    {
                        existingPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(theImage);
                        existingPost.ImageType = theImage.ContentType;
                    }

                    // when using quill rich text editor, content will never be an empty string because
                    // quill auto inserts <p><br></p>; it will replace the <br> tag with whatever is 
                    // entered by the user; so this is a check for no data being entered by user. Should
                    // refactor this into a _configuration setting?
                    if (existingPost.Content == "<p><br></p>")
                    {
                        ModelState.AddModelError("Content", "The Content field is required.");
                        ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);
                        ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
                        return View(blogPost);
                    }

                    foreach (var tagId in tagIds)
                    {
                        existingPost.Tags.Add(await _context.Tag.FindAsync(tagId));
                    }

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPost
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPost.FindAsync(id);
            _context.BlogPost.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPost.Any(e => e.Id == id);
        }
    }
}
