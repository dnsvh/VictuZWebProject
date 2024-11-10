namespace VictuZWebProject.Models
{
    public class Order
    {
        public int Id { get; set; }  // Order ID
        public string CustomerName { get; set; }  // Naam van de klant
        public string Email { get; set; }  // Email van de klant
        public DateTime OrderDate { get; set; }  // Datum van de bestelling
        public decimal TotalAmount { get; set; }  // Totaalbedrag van de bestelling
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }  // Lijst van bestelde producten

        public Order()
        {
            ShoppingCartItems = new List<ShoppingCartItem>();
        }
    }
}
