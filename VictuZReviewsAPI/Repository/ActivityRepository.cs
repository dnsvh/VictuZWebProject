using VictuZReviewsAPI.Data;
using VictuZReviewsAPI.Interfaces;
using VictuZReviewsAPI.Models;

namespace VictuZReviewsAPI.Repository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly VictuZContext victuZContext;

        public ActivityRepository(VictuZContext victuZContext)
        {
            this.victuZContext = victuZContext;
        }

        public ICollection<Activity> GetActivities()
        {
            return victuZContext.Activities.OrderBy(a => a.Id).ToList();
        }
    }
}
