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

            return View(suggestionViewModels); // Pass the mapped view models to the view
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
                    // Set Author as "FirstName LastName"
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

        // GET: Suggestions/ConvertToActivity/5
        [HttpGet]
        public async Task<IActionResult> ConvertToActivityPage(int id)
        {
            // Fetch the suggestion by ID
            var suggestion = await _context.Suggestion.FindAsync(id);
            if (suggestion == null)
            {
                return NotFound(); // Return a 404 if the suggestion doesn't exist
            }

            // Initialize the ViewModel with suggestion details
            var model = new CreateActivityViewModel
            {
                SuggestionId = suggestion.Id,
                Name = suggestion.Title,  // Automatically fill in the Name
                Body = suggestion.Body,    // Automatically fill in the Body
                Location = "",             // This can be filled by the user
                MaxCapacity = 10,          // Default value, can be updated
                DateDue = DateTime.UtcNow.AddDays(7) // Default to 7 days from now
            };

            return View(model); // Return the view with the model
        }

        // POST: Suggestions/ConvertToActivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToActivity(CreateActivityViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Activity object based on the ViewModel
                var activity = new Activity
                {
                    Name = model.Name,
                    Body = model.Body,
                    Location = model.Location,
                    Registered = 0, // Initially, no one is registered
                    MaxCapacity = model.MaxCapacity,
                    DatePublished = DateTime.UtcNow,
                    DateDue = model.DateDue // Use the user-specified date
                };

                // Add the new activity to the context
                _context.Activity.Add(activity);
                await _context.SaveChangesAsync();

                // Optionally, remove the suggestion
                // var suggestion = await _context.Suggestion.FindAsync(model.SuggestionId);
                // if (suggestion != null)
                // {
                //     _context.Suggestion.Remove(suggestion);
                //     await _context.SaveChangesAsync();
                // }

                return RedirectToAction(nameof(Index)); // Redirect to index or another relevant page
            }

            // If model state is invalid, return to the view with the model
            return View(model);
        }

    }
}



