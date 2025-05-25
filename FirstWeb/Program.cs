using FirstWeb.Repository;
using Microsoft.EntityFrameworkCore;

namespace FirstWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Connection db
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
            });


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //Seeding Data
            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            SeedData.SeedingDataa(context);

            app.Run();
        }
    }
}
