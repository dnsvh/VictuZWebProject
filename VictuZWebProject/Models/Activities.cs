namespace VictuZ_Lars.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Location { get; set; }
        public string ?ImageUrl { get; set; }
        public int Registered { get; set; }
        public int MaxCapacity { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateDue { get; set; }

        public void CreateActivity()
        {

        }

        public void ReadActivity()
        {

        }

        public void UpdateActivity()
        {

        }

        public void DeleteActivity()
        {

        }

        public void LogIn()
        {

        }

    }
}
