namespace VictuZWebProject.Models
    {
        public class Order
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public List<ShoppingCartItem> ShoppingCartItems { get; set; }
            public Order()
            {
                ShoppingCartItems = new List<ShoppingCartItem>();
            }
        }
    }
