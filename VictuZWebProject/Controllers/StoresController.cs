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
using VictuZWebProject.Services;

namespace VictuZWebProject.Controllers
{
    public class StoresController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly VictuZ_Lars_Db _context;
        private readonly ShoppingCartService _shoppingCartService;

        public StoresController(IWebHostEnvironment webHostEnvironment, VictuZ_Lars_Db context, ShoppingCartService shoppingCartService)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _shoppingCartService = shoppingCartService;
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryList = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToListAsync();

            return View();
        }

        // GET: Stores
        public async Task<IActionResult> Index(string searchString)
        {

            ViewData["CurrentFilter"] = searchString;

            var stores = from s in _context.Store
                         select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                stores = stores.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            return View(await stores.ToListAsync());
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
            ViewBag.CategoryList = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToListAsync();
            return View(store);
        }

        // GET: Stores/Edit/5
        [Authorize(Roles = "Admin,Staff")]
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

            ViewBag.CategoryList = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToListAsync();

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
            ViewBag.CategoryList = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToListAsync();
            return View(store);
        }

        // GET: Stores/Delete/5
        [Authorize(Roles = "Admin,Staff")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateCategory([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateCategoryPartial", category);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult CreateCategoryForm()
        {
            return PartialView("_CreateCategoryPartial", new Category());
        }



        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
