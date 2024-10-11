using VictuZReviewsAPI.Models;
using VictuZReviewsAPI.Data;

namespace VictuZReviewsAPI
{
    public class Seed
    {
        /*
        Dit ging volledig fout met de migrations en Rick zei dat ik beter geen seeding kon gebruiken :D


        private readonly Data.AppContext applicationDbContext;
        public Seed(Data.AppContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public void SeedDataContext()
        {
            if (!applicationDbContext.ActivityAttendees.Any())
            {
                var activityAttendees = new List<ActivityAttendee>
                {
                    new ActivityAttendee
                    {
                        Activity = new Activity()
                        {
                            Name = "Stickers maken",
                            Description = "Stickers maken van hoge kwaliteit!",
                            Timestamp = DateTime.Now,
                            Host = new Models.Host()
                            {
                                FirstName = "Miel",
                                LastName = "Noelanders"
                            },
                            Reviews = new List<Review>()
                            {
                                new Review()
                                {
                                    Title = "Wauw wat leuk!",
                                    Content = "Ik heb nog nooit zoveel plezier gehad, wat een geweldige activiteit, zeker niet geschreven door Miel.",
                                    Reviewer = new Reviewer()
                                    {
                                        FirstName = "Nietmiel",
                                        LastName = "Nietnoelanders"
                                    }
                                }
                            }
                        },
                        Attendee = new Attendee()
                        {
                            FirstName = "Attendee 1",
                            LastName = "Attendee 1"
                        }
                    },
                    new ActivityAttendee
                    {
                        Activity = new Activity()
                        {
                            Name = "Slapen",
                            Description = "Ik wil naar bed gaan",
                            Timestamp = DateTime.Now,
                            Host = new Models.Host()
                            {
                                FirstName = "Denise",
                                LastName = "van Herten"
                            },
                            Reviews = new List<Review>()
                            {
                                new Review()
                                {
                                    Title = "Goed geslapen",
                                    Content = "Ik heb meer dan twee uur geslapen, wat een leuke activiteit",
                                    Reviewer = new Reviewer()
                                    {
                                        FirstName = "Tim",
                                        LastName = "Oehiervoor"
                                    }
                                }
                            }
                        },
                        Attendee = new Attendee()
                        {
                            FirstName = "Attendee 2",
                            LastName = "Attendee 2"
                        }
                    },
                    new ActivityAttendee
                    {
                        Activity = new Activity()
                        {
                            Name = "Wandelen",
                            Description = "Making way down townnnnn",
                            Timestamp = DateTime.Now,
                            Host = new Models.Host()
                            {
                                FirstName = "Kayleigh",
                                LastName = "van Hoof"
                            },
                            Reviews = new List<Review>()
                            {
                                new Review()
                                {
                                    Title = "Wat een rotactiviteit",
                                    Content = "Ik hou niet van wandelen :(",
                                    Reviewer = new Reviewer()
                                    {
                                        FirstName = "Wan",
                                        LastName = "Delhaat"
                                    }
                                }
                            }
                        },
                        Attendee = new Attendee()
                        {
                            FirstName = "Attendee 3",
                            LastName = "Attendee 3"
                        }
                    }
                };
                applicationDbContext.ActivityAttendees.AddRange(activityAttendees);
                applicationDbContext.SaveChanges();
            }


        }
        */
    }
}
