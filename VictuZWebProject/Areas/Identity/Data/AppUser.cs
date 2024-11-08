using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VictuZ_Lars.Models;
using VictuZWebProject.Models;



namespace VictuZWebProject.Areas.Identity.Data;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Activity> Activities { get; set; }
    public ICollection<UserRegistration> UserRegistrations { get; set; }
}

