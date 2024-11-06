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
using VictuZWebProject.Areas.Identity.Data;
using VictuZ_Lars.Models;

namespace VictuZWebProject.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly UserManager<AppUser> _userManager;

        public SuggestionsController(VictuZ_Lars_Db context, UserManager<Areas.Identity.Data.AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Suggestions
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the suggestions
            var suggestions = await _context.Suggestion.ToListAsync();

            // Map Suggestions to SuggestionViewModel
            var suggestionViewModels = suggestions.Select(s => new SuggestionViewModel
            {
                Suggestion = s,
                HasUserLiked = _context.SuggestionLike.Any(sl => sl.UserId == userId && sl.SuggestionId == s.Id)
            }).ToList();

            return View(suggestionViewModels);
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
                // Get the current user
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Author = name
                    suggestion.Author = $"{user.FirstName} {user.LastName}";
                }
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

            // Get user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user has liked
            var existingLike = await _context.SuggestionLike
                .FirstOrDefaultAsync(l => l.UserId == userId && l.SuggestionId == Id);

            var suggestion = await _context.Suggestion.FirstOrDefaultAsync(m => m.Id == Id);
            if (suggestion == null)
            {
                return NotFound();
            }

            if (existingLike != null)
            {
                // Unlike
                _context.SuggestionLike.Remove(existingLike);
                suggestion.Likes -= 1;
            }
            else
            {
                // Like
                var like = new SuggestionLike
                {
                    UserId = userId,
                    SuggestionId = (int)Id
                };

                _context.SuggestionLike.Add(like);
                suggestion.Likes += 1;
            }

            // Update
            _context.Suggestion.Update(suggestion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Suggestions/ConvertToActivity/5
        [HttpGet]
        public async Task<IActionResult> ConvertToActivityPage(int id)
        {
            var suggestion = await _context.Suggestion.FindAsync(id);
            if (suggestion == null)
            {
                return NotFound();
            }

            var model = new CreateActivityViewModel
            {
                SuggestionId = suggestion.Id,
                Name = suggestion.Title,
                Organizer = suggestion.Author,  // Set Organizer based on Suggestion's Author
                Body = suggestion.Body,
                Location = "",
                MaxCapacity = 10,
                DateDue = DateTime.UtcNow.AddDays(7)
            };

            return View("ConvertToActivityPage", model);
        }


        // POST: Suggestions/ConvertToActivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToActivity(CreateActivityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activity = new Activity
                {
                    Name = model.Name,
                    Body = model.Body,
                    Location = model.Location,
                    Organizer = model.Organizer,  // Assign Organizer from the ViewModel
                    Registered = 0,
                    MaxCapacity = model.MaxCapacity,
                    DatePublished = DateTime.UtcNow,
                    DateDue = model.DateDue
                };

                _context.Activity.Add(activity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("ConvertToActivity", model);
        }

    }
}



