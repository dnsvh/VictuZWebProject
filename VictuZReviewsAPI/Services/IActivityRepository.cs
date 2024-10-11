using VictuZReviewsAPI.Models;

namespace VictuZReviewsAPI.Interfaces
{
    public interface IActivityRepository
    {
        ICollection<Activity> GetActivities();
    }
}
