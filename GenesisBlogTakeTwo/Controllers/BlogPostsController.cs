﻿#nullable disable
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

namespace GenesisBlogTakeTwo.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context, 
                                   IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            var blogPost = await _context.BlogPost.Include(b => b.Tags).ToListAsync();
            return View(blogPost);
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text");
            ViewData["BlogPostStatesList"] = new SelectList(Enum.GetValues(typeof(BlogPostState)).Cast<BlogPostState>().ToList());
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,BlogPostState")] BlogPost blogPost, IFormFile theImage, List<int> tagIds)
        {
            if (ModelState.IsValid)
            {
                // verigy that IFormFile isn't null before interacting with it
                if(theImage is not null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(theImage);
                    blogPost.ImageType = theImage.ContentType;
                }


                // when using quill rich text editor, content will never be an empty string because
                // quill auto inserts <p><br></p>; it will replace the <br> tag with whatever is 
                // entered by the user; so this is a check for no data being entered by user. Should
                // refactor this into a _configuration setting?
                if (blogPost.Content == "<p><br></p>")
                {
                    ModelState.AddModelError("Content", "Sorry, you must enter something for content");
                    return View(blogPost);
                }

                blogPost.Created = DateTime.SpecifyKind(blogPost.Created, DateTimeKind.Utc);

                // Associate any/all tags with the BlogPost
                foreach (var tagId in tagIds)
                {
                    // do something...
                    blogPost.Tags.Add(await _context.Tag.FindAsync(tagId));
                }

                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPost.Include(b => b.Tags)
                                                  .FirstOrDefaultAsync(b => b.Id == id);

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Slug,IsDeleted,BlogPostState")] BlogPost blogPost, IFormFile theImage, List<int> tagIds)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

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

                    //blogPost.Created = DateTime.SpecifyKind(blogPost.Created, DateTimeKind.Utc);
                    //blogPost.Updated = DateTime.UtcNow;

                    ////TODO: Image Service
                    if (theImage is not null)
                    {
                        existingPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(theImage);
                        existingPost.ImageType = theImage.ContentType;
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
