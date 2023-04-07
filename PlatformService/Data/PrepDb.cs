using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data 
{
    //This class isn't to be used in production, this is here for testing and seeding the database
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction = false)
        {
            //Need to use a scope in order to get access to the DB
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction = false)
        {
            if(isProduction)
            {
                Console.WriteLine("--> Applying Migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"--> Applying Migrations FAILED!: {ex}");
                }
            }

            if(!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );

                context.SaveChanges();
            }else{
                Console.WriteLine("--> We already have data");
            }
        }
    }
}