using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VictuZ_Lars.Data;
using VictuZWebProject.Areas.Identity.Data;
using VictuZWebProject.Data;

namespace VictuZWebProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("VictuZAccountDbContextConnection")
                ?? throw new InvalidOperationException("Connection string 'VictuZAccountDbContextConnection' not found.");

            builder.Services.AddDbContext<VictuZAccountDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDbContext<VictuZ_Lars_Db>();
            builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<VictuZAccountDbContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Cultuur instellen met een punt als decimaalteken
            var cultureInfo = new CultureInfo("en-US")
            {
                NumberFormat = {
                    CurrencyDecimalSeparator = ".",
                    NumberDecimalSeparator = "."
    }
            };

            // Standaard cultuur instellen voor de hele applicatie
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  // HSTS configuration
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Role seeding
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Staff", "Member", "Visitor", "Pending" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Admin user seeding
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                string adminEmail = "adminaccount@zuyd.nl";
                string adminPassword = "AdminAccount123!";
                string adminFirstName = "Admin";
                string adminLastName = "Account";

                // FindByEmailAsync to get the user
                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
                if (existingAdmin == null)
                {
                    var adminUser = new AppUser
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        FirstName = adminFirstName,
                        LastName = adminLastName,
                        EmailConfirmed = true
                    };

                    var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createAdminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }

            app.Run();
        }
    }
}

