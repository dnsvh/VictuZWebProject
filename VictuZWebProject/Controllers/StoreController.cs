using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Models;
using VictuZ_Lars.Data;

namespace VictuZWebProject.Controllers
{
    public class StoreController : Controller
    {
        private readonly VictuZ_Lars_Db _context;

        public StoreController(VictuZ_Lars_Db context)
        {
            _context = context;
        }

        // GET: StoreModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.StoreModel.ToListAsync());
        }

        // GET: StoreModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeModel = await _context.StoreModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (storeModel == null)
            {
                return NotFound();
            }

            return View(storeModel);
        }

        // GET: StoreModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StoreModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,ImagePath,Price")] StoreModel storeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(storeModel);
        }

        // GET: StoreModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeModel = await _context.StoreModel.FindAsync(id);
            if (storeModel == null)
            {
                return NotFound();
            }
            return View(storeModel);
        }

        // POST: StoreModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,ImagePath,Price")] StoreModel storeModel)
        {
            if (id != storeModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreModelExists(storeModel.ID))
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
            return View(storeModel);
        }

        // GET: StoreModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeModel = await _context.StoreModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (storeModel == null)
            {
                return NotFound();
            }

            return View(storeModel);
        }

        // POST: StoreModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeModel = await _context.StoreModel.FindAsync(id);
            if (storeModel != null)
            {
                _context.StoreModel.Remove(storeModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreModelExists(int id)
        {
            return _context.StoreModel.Any(e => e.ID == id);
        }
    }
}
