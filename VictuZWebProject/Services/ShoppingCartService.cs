using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VictuZ_Lars.Data;
using VictuZWebProject.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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
        public int GetCartItemCount()
        {
            var cart = GetOrCreateCart();
            return cart.Items.Sum(i => i.Quantity); // Tel de hoeveelheid van alle items in de winkelwagen op
        }

        public decimal GetCartTotalPrice()
        {
            var cart = GetOrCreateCart();
            return cart.TotalPrice; // Haal de totale prijs van de winkelwagen op
        }

        public void UpdateCart(int storeId, int quantity)
        {
            var cart = GetOrCreateCart(); // Haal de winkelwagen op (of maak een nieuwe als die niet bestaat)
            var store = _context.Store.FirstOrDefault(s => s.Id == storeId); // Haal het product uit de database

            if (store == null)
            {
                throw new Exception("Product niet gevonden.");
            }

            // Zoek of het product al in de winkelwagen zit
            var cartItem = cart.Items.FirstOrDefault(i => i.StoreId == storeId);

            if (cartItem != null)
            {
                // Werk de hoeveelheid bij
                cartItem.Quantity = quantity;
            }
            else
            {
                // Als het item nog niet in de winkelwagen staat, voeg het toe
                var newItem = new ShoppingCartItem
                {
                    StoreId = storeId,
                    Quantity = quantity,
                    Price = store.Price,
                };
                cart.Items.Add(newItem);
            }

            // Sla de wijzigingen op in de context (of sessie, afhankelijk van waar je de winkelwagen bewaart)
            _context.SaveChanges();
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
