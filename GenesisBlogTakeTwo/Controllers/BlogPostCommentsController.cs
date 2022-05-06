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

namespace GenesisBlogTakeTwo.Controllers
{
    public class BlogPostCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogPostCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BlogPostComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPostComment.Include(b => b.BlogPost);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPostComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComment
                .Include(b => b.BlogPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }

            return View(blogPostComment);
        }

        // GET: BlogPostComments/Create
        public IActionResult Create()
        {
            ViewData["BlogPostId"] = new SelectList(_context.BlogPost, "Id", "Abstract");
            return View();
        }

        // POST: BlogPostComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BlogPostId,Comment")] BlogPostComment blogPostComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogPostComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogPostId"] = new SelectList(_context.BlogPost, "Id", "Abstract", blogPostComment.BlogPostId);
            return View(blogPostComment);
        }

        // GET: BlogPostComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComment.FindAsync(id);
            if (blogPostComment == null)
            {
                return NotFound();
            }
            ViewData["BlogPostId"] = new SelectList(_context.BlogPost, "Id", "Abstract", blogPostComment.BlogPostId);
            return View(blogPostComment);
        }

        // POST: BlogPostComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogPostId,Comment")] BlogPostComment blogPostComment)
        {
            if (id != blogPostComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPostComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostCommentExists(blogPostComment.Id))
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
            ViewData["BlogPostId"] = new SelectList(_context.BlogPost, "Id", "Abstract", blogPostComment.BlogPostId);
            return View(blogPostComment);
        }

        // GET: BlogPostComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComment
                .Include(b => b.BlogPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }

            return View(blogPostComment);
        }

        // POST: BlogPostComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPostComment = await _context.BlogPostComment.FindAsync(id);
            _context.BlogPostComment.Remove(blogPostComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostCommentExists(int id)
        {
            return _context.BlogPostComment.Any(e => e.Id == id);
        }
    }
}
