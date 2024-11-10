namespace VictuZWebProject.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Koppeling naar de gebruiker
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }

}
