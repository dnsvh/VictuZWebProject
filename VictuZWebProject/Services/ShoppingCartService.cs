using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VictuZ_Lars.Data;
using VictuZWebProject.Models;

namespace VictuZWebProject.Services
{
    public class ShoppingCartService
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartService(VictuZ_Lars_Db context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        // Haal de winkelwagen van de ingelogde gebruiker op
        public ShoppingCart GetOrCreateCart()
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier); // Haal de userId op
            if (userId == null) return null;

            var cart = _context.ShoppingCarts.Include(c => c.Items)
                                             .FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId };
                _context.ShoppingCarts.Add(cart);
                _context.SaveChanges();
            }
            return cart;
        }


        // Voeg een product toe aan de winkelwagen
        public void AddToCart(int storeId, int quantity)
        {
            var cart = GetOrCreateCart(); // Haal de winkelwagen op (of maak een nieuwe als die niet bestaat)
            var store = _context.Store.FirstOrDefault(s => s.Id == storeId); // Haal de winkel uit de database
            if (store == null || cart == null) return;

            var existingItem = cart.Items.FirstOrDefault(i => i.StoreId == storeId); // Kijk of het product al in de winkelwagen zit
            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Verhoog het aantal van dit product
            }
            else
            {
                var newItem = new ShoppingCartItem
                {
                    StoreId = storeId,
                    Quantity = quantity,
                    Price = store.Price,
                    Name = store.Name
                };
                cart.Items.Add(newItem);
            }

            // Update het aantal producten in de sessie
            var cartItemCount = cart.Items.Sum(i => i.Quantity);
            _httpContextAccessor.HttpContext.Session.SetInt32("CartItemCount", cartItemCount);

            _context.SaveChanges();
        }

        // Verkrijg de winkelwagenitems
        public List<ShoppingCartItem> GetCartItems()
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return new List<ShoppingCartItem>();

            var cart = _context.ShoppingCarts.Include(c => c.Items)
                                             .FirstOrDefault(c => c.UserId == userId);
            return cart?.Items ?? new List<ShoppingCartItem>();
        }

        public int GetCartItemCount()
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return 0;

            var cart = _context.ShoppingCarts.Include(c => c.Items)
                                             .FirstOrDefault(c => c.UserId == userId);

            var cartItemCount = cart?.Items.Sum(i => i.Quantity) ?? 0;

            // Update session zodat het actuele aantal altijd beschikbaar is
            _httpContextAccessor.HttpContext.Session.SetInt32("CartItemCount", cartItemCount);

            return cartItemCount;
        }

        public void RemoveCartItem(int storeId)
        {
            // Verkrijg de huidige winkelwagen of maak een nieuwe als deze niet bestaat
            var cart = GetOrCreateCart();

            // Zoek het item in de winkelwagen op basis van de storeId (of een andere identifier)
            var itemToRemove = cart.Items.FirstOrDefault(i => i.StoreId == storeId);

            // Als het item gevonden is, verwijder het uit de winkelwagen
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
            }

            // Werk de sessie bij met het nieuwe totaal aantal producten
            var cartItemCount = cart.Items.Sum(i => i.Quantity);
            _httpContextAccessor.HttpContext.Session.SetInt32("CartItemCount", cartItemCount);

            // Sla de veranderingen op in de database (indien nodig)
            _context.SaveChanges();
        }


    }

}
