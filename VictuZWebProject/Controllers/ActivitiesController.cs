using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZ_Lars.Data;
using VictuZ_Lars.Models;
using VictuZWebProject.Data;
using VictuZWebProject.Models;

namespace VictuZ_Lars.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly VictuZAccountDbContext _usercontext;

        public ActivitiesController(VictuZ_Lars_Db context, VictuZAccountDbContext usercontext)
        {
            _context = context;
            _usercontext = usercontext;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            // Haal huidige tijd op
            var currentTime = DateTime.UtcNow;

            // Controleer of de gebruiker de rol "Member" heeft
            bool isMember = User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin");


            // Haal de activiteiten op en filter activiteiten met `OnlyMembers = true` voor niet-leden
            var activitiesQuery = _context.Activity.AsQueryable();

            //OnlyMembers
            if (!isMember)
            {
                // Als de gebruiker geen van de drie rollen heeft, filter dan de activiteiten die alleen voor leden zijn
                activitiesQuery = activitiesQuery.Where(a => !a.OnlyMembers);
            }

            //MembersOnlyVisibilityEnd
            if (!isMember)
            {
                // Voor niet-leden filteren we activiteiten die alleen voor members zichtbaar zijn
                activitiesQuery = activitiesQuery.Where(a =>
                    (!a.MembersPreRegistration ||
                     (a.MembersOnlyVisibilityEnd.HasValue && currentTime >= a.MembersOnlyVisibilityEnd.Value)));
            }


            // Haal de gefilterde activiteiten op en sorteer op datum
            var activities = await activitiesQuery
                .OrderBy(a => a.DateDue)
                .ToListAsync();

            // Haal de registratiegegevens van de gebruiker op
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRegistrations = await _context.UserRegistration
                .Where(r => r.UserId == userId)
                .Select(r => r.ActivityId)
                .ToListAsync();

            // Bouw het viewmodel
            var viewModel = activities.Select(a => new ActivityViewModel
            {
                Activity = a,
                IsUserRegistered = userRegistrations.Contains(a.ActivityId),
                AvailableForMembers = CalculateAvailableForMembers(a),
                AvailableForNonMembers = CalculateAvailableForNonMembers(a)
            }).ToList();

            return View(viewModel);
        }

        private int CalculateAvailableForMembers(Activity activity)
        {
            if (activity.MembersOnlyCapacity.HasValue)
            {
                // Tel het aantal geregistreerde Members, Staff en Admins
                int membersIngeschreven = _context.UserRegistration
                    .Count(r => r.ActivityId == activity.ActivityId &&
                                (User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin")));

                return Math.Max(0, activity.MembersOnlyCapacity.Value - membersIngeschreven);
            }
            return 0; // Geen gereserveerde plekken
        }

        private int CalculateAvailableForNonMembers(Activity activity)
        {
            if (activity.MembersOnlyCapacity.HasValue)
            {
                // Bereken de beschikbare plekken voor Visitors
                int totalMembersRegistered = _context.UserRegistration
                    .Count(r => r.ActivityId == activity.ActivityId &&
                                (User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin")));

                int visitorCapacity = activity.MaxCapacity - activity.MembersOnlyCapacity.Value; // Totale capaciteit minus gereserveerde plekken

                int visitorsIngeschreven = _context.UserRegistration
                    .Count(r => r.ActivityId == activity.ActivityId &&
                                !User.IsInRole("Member") &&
                                !User.IsInRole("Staff") &&
                                !User.IsInRole("Admin"));

                return Math.Max(0, visitorCapacity - visitorsIngeschreven);
            }
            return Math.Max(0, activity.MaxCapacity - activity.Registered); // Geen gereserveerde plekken
        }



        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }

            // Controleer of de activiteit alleen zichtbaar is voor members
            if (activity.OnlyMembers && !(User.IsInRole("Member") || User.IsInRole("Admin") || User.IsInRole("Staff")))
            {
                return Forbid(); // Weiger toegang als de gebruiker geen "Member", "Admin", of "Staff" is
            }


            return View(activity);
        }

        //public async Task<IActionResult> Register(int? Id)
        //{
        //    if (Id == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //
        //    var existingRegistration = await _context.UserRegistration
        //        .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == Id);
        //
        //    if (existingRegistration != null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //
        //    var registration = new UserRegistration
        //    {
        //        UserId = userId,
        //        ActivityId = (int)Id,
        //    };
        //
        //
        //    _context.UserRegistration.Add(registration);
        //
        //    var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
        //    if (activity == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    if (activity.Registered < activity.MaxCapacity)
        //    {
        //        // Update the database
        //        activity.Registered += 1;
        //        _context.Activity.Update(activity);
        //        _context.UserRegistration.Add(registration);
        //        await _context.SaveChangesAsync();
        //
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        // GET: Activities/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        // Modify the Create method to accept an image file
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([Bind("ActivityId,Name,Body,Organizer,Location,MaxCapacity,DatePublished,DateDue,OnlyMembers,MembersOnlyVisibilityEnd,MembersPreRegistration,MembersOnlyCapacity")] Activity activity, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Check if an image file is uploaded
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Generate a unique filename for the uploaded image
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    var filePath = Path.Combine("wwwroot/uploads", uniqueFileName);

                    // Save the image file to wwwroot/uploads directory
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Set the ImageUrl property to the saved file path
                    activity.ImageUrl = $"/uploads/{uniqueFileName}";
                }

                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,Name,Body,Organizer,Location,MaxCapacity,DatePublished,DateDue,OnlyMembers,MembersOnlyVisibilityEnd,MembersPreRegistration,MembersOnlyCapacity")] Activity activity, IFormFile imageFile)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload if a new file is uploaded
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                        var filePath = Path.Combine("wwwroot/uploads", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        activity.ImageUrl = $"/uploads/{uniqueFileName}";
                    }

                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ActivityId))
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
            return View(activity);
        }



        // GET: Activities/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activity.FindAsync(id);
            if (activity != null)
            {
                _context.Activity.Remove(activity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }


        public async Task<IActionResult> Register(int? Id, string returnUrl = null)
        {

            if (Id == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Get the current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isMember = User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin");
            bool isVisitor = User.IsInRole("Visitor");

            // Controleer of de activiteit bestaat
            var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (activity == null)
            {
                return NotFound();
            }

            // Check if liked
            var existingRegister = await _context.UserRegistration
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ActivityId == Id);

            var registration = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (registration == null)
            {
                return NotFound();
            }

            if (existingRegister != null)
            {
                // Verwijder registratie als de gebruiker al geregistreerd is
                _context.UserRegistration.Remove(existingRegister);
                activity.Registered -= 1;
            }
            else
            {
                // Bereken het aantal huidige registraties
                int memberCount = _context.UserRegistration.Count(r => r.ActivityId == Id &&
                                  (User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin")));
                int visitorCount = _context.UserRegistration.Count(r => r.ActivityId == Id && User.IsInRole("Visitor"));

                if (isVisitor)
                {
                    // Bezoekers kunnen zich alleen registreren als er plaatsen over zijn buiten de gereserveerde capaciteit
                    if (visitorCount >= activity.MaxCapacity - activity.MembersOnlyCapacity)
                    {
                        return BadRequest("Geen beschikbare plaatsen voor bezoekers.");
                    }
                }
                else if (isMember)
                {
                    // Leden kunnen zich registreren zolang MaxCapacity niet bereikt is
                    if (activity.Registered >= activity.MaxCapacity)
                    {
                        return BadRequest("De activiteit zit vol.");
                    }
                }

                // Voeg registratie toe
                var reg = new UserRegistration
                {
                    UserId = userId,
                    ActivityId = (int)Id
                };

                _context.UserRegistration.Add(reg);
                activity.Registered += 1;
            }

            // Update
            _context.Activity.Update(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Activities");
        }

        public async Task<IActionResult> RegisteredUsers(int id)
        {
            ViewBag.ActivityId = id;

            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            var registeredUserIds = await _context.UserRegistration
                .Where(r => r.ActivityId == id)
                .Select(r => r.UserId)
                .ToListAsync();

            var registeredUsers = await _usercontext.Users
                .Where(u => registeredUserIds.Contains(u.Id))
                .Select(u => new UserRegistrationViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .ToListAsync();

            return View(registeredUsers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteRegistration(string userId, int activityId)
        {
            // Find the registration based on userId and activityId
            var registration = await _context.UserRegistration
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == activityId);

            if (registration != null)
            {
                // Remove the registration
                _context.UserRegistration.Remove(registration);

                // Decrement the registered count for the activity
                var activity = await _context.Activity.FindAsync(activityId);
                if (activity != null)
                {
                    activity.Registered -= 1;
                    _context.Activity.Update(activity);
                }

                await _context.SaveChangesAsync();
            }

            // Redirect back to the RegisteredUsers view, passing the activityId
            return RedirectToAction("RegisteredUsers", new { id = activityId });
        }













    }
}
