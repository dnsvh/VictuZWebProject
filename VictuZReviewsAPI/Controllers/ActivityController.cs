using Microsoft.AspNetCore.Mvc;
using VictuZReviewsAPI.Repository;
using VictuZReviewsAPI.Interfaces;
using System.Diagnostics;

namespace VictuZReviewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly IActivityRepository activityRepository;
        public ActivityController(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository;


        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<Activity>))]
        public IActionResult GetActivities()
        {
            var activities = activityRepository.GetActivities();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(activities);
        }
    }
}
