
using Microsoft.EntityFrameworkCore;
using VictuZReviewsAPI.Data;
using VictuZReviewsAPI.Interfaces;
using VictuZReviewsAPI.Repository;

namespace VictuZReviewsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // builder.Services.AddTransient<Seed>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // references the database context using the default connection string in appsettings
            builder.Services.AddDbContext<Data.VictuZContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            /*
            // Seed the database with data
            if (args.Length == 1 && args[0].ToLower() == "seeddata")
                SeedData(app);

            void SeedData(IHost app)
            {
                var scopeFactory = app.Services.GetService<IServiceScopeFactory>();

                using (var scope = scopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<Seed>();
                    service.SeedDataContext();
                }
            }
            */

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
