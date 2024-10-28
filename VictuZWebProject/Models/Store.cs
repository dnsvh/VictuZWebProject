namespace VictuZWebProject.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ? Size { get; set; }
        public float Price { get; set; }
        public string ? ImageUrl { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
    }
}
