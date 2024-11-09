using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VictuZ_Lars.Data;
using VictuZWebProject.Areas.Identity.Data;
using VictuZWebProject.Data;
using VictuZWebProject.Services;

namespace VictuZWebProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Voeg sessieconfiguratie toe
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Stel de time-out in (bijvoorbeeld 30 minuten)
                options.Cookie.HttpOnly = true; // Voor meer veiligheid
                options.Cookie.IsEssential = true; // Zorgt ervoor dat de cookie altijd wordt verzonden
            });

            // Haal de connectiestring op voor de database
            var connectionString = builder.Configuration.GetConnectionString("VictuZAccountDbContextConnection")
                ?? throw new InvalidOperationException("Connection string 'VictuZAccountDbContextConnection' not found.");

            // Registreer DbContext voor de Account en de VictuZ_Lars DbContext
            builder.Services.AddDbContext<VictuZAccountDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDbContext<VictuZ_Lars_Db>();

            // Configureer Identity met de juiste rol en UserManager
            builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<VictuZAccountDbContext>();

            builder.Services.AddScoped<ShoppingCartService>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Cultuurinstellingen voor het gebruik van punt als decimaalteken
            var cultureInfo = new CultureInfo("en-US")
            {
                NumberFormat = {
                    CurrencyDecimalSeparator = ".",
                    NumberDecimalSeparator = "."
                }
            };
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Als de omgeving niet Development is, configureer dan de exception handler en HSTS
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Voeg de sessie middleware toe vóór andere middleware
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Configureer de routes voor controllers en Razor Pages
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Role seeding (voeg standaardrollen toe als ze nog niet bestaan)
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

                // Correctly await FindByEmailAsync to get the user
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

            // Start de applicatie
            app.Run();
        }
    }
}
