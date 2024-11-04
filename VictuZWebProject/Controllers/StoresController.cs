using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Models;
using VictuZ_Lars.Data;
using Microsoft.AspNetCore.Authorization;

namespace VictuZWebProject.Controllers
{
    public class StoresController : Controller
    {
        private readonly VictuZ_Lars_Db _context;

        public StoresController(VictuZ_Lars_Db context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            // Current Categories
            var categories = new List<string> {"Clothing", "Stickers", "Miscellanious" };
            ViewBag.CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

            return View();
        }


        // GET: Stores
        public async Task<IActionResult> Index()
        {


            // Haal de activiteiten op en filter activiteiten met `OnlyMembers = true` voor niet-leden
            var storesQuery = _context.Store.AsQueryable();


            var products = await storesQuery.ToListAsync();
            return View(products);
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            // Controleer of de activiteit alleen zichtbaar is voor members
            if (store.MemberPlusProduct && !(User.IsInRole("Member") || User.IsInRole("Admin") || User.IsInRole("Staff")))
            {
                return Forbid(); // Weiger toegang als de gebruiker geen "Member", "Admin", of "Staff" is
            }

            return View(store);
        }


        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Size,Price,ImageUrl,Category,Stock,MemberPlusProduct")] Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            // Current Categories
            var categories = new List<string> { "Clothing", "Stickers", "Miscellanious" };
            ViewBag.CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Size,Price,ImageUrl,Category,Stock,MemberPlusProduct")] Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
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
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var store = await _context.Store.FindAsync(id);
            if (store != null)
            {
                _context.Store.Remove(store);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
