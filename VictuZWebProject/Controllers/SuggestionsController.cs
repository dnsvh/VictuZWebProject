using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Models;
using VictuZ_Lars.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace VictuZWebProject.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly VictuZ_Lars_Db _context;

        public SuggestionsController(VictuZ_Lars_Db context)
        {
            _context = context;
        }

        // GET: Suggestions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Suggestion.ToListAsync());
        }

        // GET: Suggestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestion = await _context.Suggestion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suggestion == null)
            {
                return NotFound();
            }

            return View(suggestion);
        }

        // GET: Suggestions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suggestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body")] Suggestion suggestion)
        {
            if (ModelState.IsValid)
            {
                suggestion.Author = User.Identity.Name;
                suggestion.Likes = 0;
                _context.Add(suggestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suggestion);
        }

        // GET: Suggestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestion = await _context.Suggestion.FindAsync(id);
            if (suggestion == null)
            {
                return NotFound();
            }
            return View(suggestion);
        }

        // POST: Suggestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Body,Likes")] Suggestion suggestion)
        {
            if (id != suggestion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suggestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuggestionExists(suggestion.Id))
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
            return View(suggestion);
        }

        // GET: Suggestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suggestion = await _context.Suggestion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suggestion == null)
            {
                return NotFound();
            }

            return View(suggestion);
        }

        // POST: Suggestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suggestion = await _context.Suggestion.FindAsync(id);
            if (suggestion != null)
            {
                _context.Suggestion.Remove(suggestion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuggestionExists(int id)
        {
            return _context.Suggestion.Any(e => e.Id == id);
        }


        // Like suggestion 
        public async Task<IActionResult> Like(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user has already liked this suggestion
            var existingLike = await _context.SuggestionLike
                .FirstOrDefaultAsync(l => l.UserId == userId && l.SuggestionId == Id);

            var suggestion = await _context.Suggestion.FirstOrDefaultAsync(m => m.Id == Id);
            if (suggestion == null)
            {
                return NotFound();
            }

            if (existingLike != null)
            {
                // User has already liked this suggestion, so remove the like (Unlike)
                _context.SuggestionLike.Remove(existingLike);
                suggestion.Likes -= 1; // Decrement the like count
            }
            else
            {
                // User hasn't liked this suggestion yet, so add the like
                var like = new SuggestionLike
                {
                    UserId = userId,
                    SuggestionId = (int)Id
                };

                _context.SuggestionLike.Add(like);
                suggestion.Likes += 1; // Increment the like count
            }

            // Update the suggestion and save changes to the database
            _context.Suggestion.Update(suggestion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
