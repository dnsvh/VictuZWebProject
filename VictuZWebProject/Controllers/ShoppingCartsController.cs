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
using Microsoft.Extensions.Logging;


namespace VictuZWebProject.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly ILogger<ShoppingCartsController> _logger;


        public ShoppingCartsController(VictuZ_Lars_Db context, ShoppingCartService shoppingCartService, ILogger<ShoppingCartsController> logger)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        [HttpPost]
        [Route("ShoppingCarts/UpdateCart/{storeId}")]
        public IActionResult UpdateCart(int storeId, int quantity)
        {
            try
            {
                // Update de winkelwagen met de nieuwe hoeveelheid
                _shoppingCartService.UpdateCart(storeId, quantity);

                // Verkrijg het totaal aantal items en de totale prijs
                var cartItemCount = _shoppingCartService.GetCartItemCount();
                var cartTotalPrice = _shoppingCartService.GetCartTotalPrice();

                // Geef de resultaten terug naar de frontend
                return Json(new { success = true, cartItemCount, cartTotalPrice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het bijwerken van de winkelwagen");
                return Json(new { success = false, message = "Er is een fout opgetreden bij het bijwerken van de winkelwagen." });
            }
        }



        [HttpPost]
        public IActionResult AddToCart(int storeId, int quantity)
        {
            _shoppingCartService.AddToCart(storeId, quantity);
            var cartItemCount = _shoppingCartService.GetCartItems().Count; // Haal het aantal winkelwagentjes op
            return Json(new { success = true, cartItemCount = cartItemCount, message = "Product toegevoegd aan winkelwagen!" });
        }

        public IActionResult RemoveFromCart(int storeId)
        {
            // Verwijder het product uit de winkelwagen
            _shoppingCartService.RemoveCartItem(storeId);

            // Redirect naar de winkelwagenpagina of waar je ook heen wilt na het verwijderen
            return RedirectToAction("Index", "ShoppingCarts");
        }


        public IActionResult Index()
        {
            // Haal het actuele aantal items op uit de winkelwagen
            var cartItemCount = _shoppingCartService.GetCartItemCount();

            // Zet het aantal items in de winkelwagen in ViewData zodat het beschikbaar is voor de View
            ViewData["CartItemCount"] = cartItemCount;

            // Verkrijg de items van de winkelwagen
            var items = _shoppingCartService.GetCartItems();

            return View(items);
        }



        // GET: ShoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId")] ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartExists(shoppingCart.Id))
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
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart != null)
            {
                _context.ShoppingCarts.Remove(shoppingCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCarts.Any(e => e.Id == id);
        }
    }
}
