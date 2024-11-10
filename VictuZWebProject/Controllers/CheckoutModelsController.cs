using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Models;
using VictuZ_Lars.Data;
using VictuZWebProject.Services;

namespace VictuZWebProject.Controllers
{
    public class CheckoutModelsController : Controller
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly ShoppingCartService _shoppingCartService;

        public CheckoutModelsController(VictuZ_Lars_Db context, ShoppingCartService shoppingCartService)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;
        }

        // GET: CheckoutModels
        public IActionResult Index()
        {
            var cartItems = _shoppingCartService.GetCartItems();
            var subtotal = cartItems.Sum(i => i.Quantity * i.Price);
            var discount = 0.00m;      // Voorbeeld: geen korting

            var totalAmount = subtotal - discount;

            var checkoutModel = new CheckoutModel
            {
                ShoppingCartItems = cartItems,
                Subtotal = subtotal,
                Discount = discount,
                TotalAmount = totalAmount
            };

            checkoutModel.TotalAmount = checkoutModel.ShoppingCartItems.Sum(item => item.Price * item.Quantity);

            return View(checkoutModel);
        }


        [HttpPost]
        public IActionResult Index(CheckoutModel model)
        {
            if (ModelState.IsValid)
            {
                // Verwerk betaling en andere logica
                // Bijvoorbeeld: sla de bestelling op, stuur bevestiging naar de klant, etc.
                return RedirectToAction("Confirmation");
            }

            // Bij een fout in het model herlaad het formulier met fouten
            return View(model);
        }


        // GET: CheckoutModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutModel = await _context.CheckoutModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkoutModel == null)
            {
                return NotFound();
            }

            return View(checkoutModel);
        }

        // GET: CheckoutModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CheckoutModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,Email,Subtotal,Discount,TotalAmount")] CheckoutModel checkoutModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(checkoutModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(checkoutModel);
        }

        // GET: CheckoutModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutModel = await _context.CheckoutModels.FindAsync(id);
            if (checkoutModel == null)
            {
                return NotFound();
            }
            return View(checkoutModel);
        }

        // POST: CheckoutModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,Email,Subtotal,Discount,TotalAmount")] CheckoutModel checkoutModel)
        {
            if (id != checkoutModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkoutModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckoutModelExists(checkoutModel.Id))
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
            return View(checkoutModel);
        }

        // GET: CheckoutModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutModel = await _context.CheckoutModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkoutModel == null)
            {
                return NotFound();
            }

            return View(checkoutModel);
        }

        // POST: CheckoutModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkoutModel = await _context.CheckoutModels.FindAsync(id);
            if (checkoutModel != null)
            {
                _context.CheckoutModels.Remove(checkoutModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutModelExists(int id)
        {
            return _context.CheckoutModels.Any(e => e.Id == id);
        }
    }
}
