namespace VictuZWebProject.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int StoreId { get; set; } // Koppeling naar het product
        public int Quantity { get; set; } // Aantal van dit product
        public decimal Price { get; set; } // Prijs van het product
        public string Name { get; set; } // Naam van het product

        public ShoppingCart ShoppingCart { get; set; } // Koppeling naar winkelwagen
        public Store Store { get; set; } // Koppeling naar product
        public decimal TotalPrice => Price * Quantity;
    }

}
