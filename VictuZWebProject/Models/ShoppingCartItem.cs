namespace VictuZWebProject.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int StoreId { get; set; } 
        public int Quantity { get; set; } 
        public decimal Price { get; set; }
        public string Name { get; set; } 

        public ShoppingCart ShoppingCart { get; set; }
        public Store Store { get; set; } 
        public decimal TotalPrice => Price * Quantity;
    }

}
