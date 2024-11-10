using System.ComponentModel.DataAnnotations;

namespace VictuZWebProject.Models
{
    public class CheckoutModel
    {
        public int Id { get; set; }  // Unieke identificatie voor de bestelling

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Email { get; set; }


        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }

    }
}

