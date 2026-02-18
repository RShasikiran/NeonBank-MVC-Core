using Microsoft.EntityFrameworkCore;
using NeonBank.Data;

namespace NeonBank
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. SERVICES
            builder.Services.AddControllersWithViews();

            // SQL Server Connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Cookie Authentication Configuration
            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "NeonBank.Cookie";
                    config.LoginPath = "/Account/Login";
                    config.AccessDeniedPath = "/Account/Login";
                    config.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            // Add session support for flash messages
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // 2. MIDDLEWARE PIPELINE
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // ✅ Required for CSS/JS to load
            app.UseRouting();
            app.UseSession();

            // IMPORTANT: Authentication must come BEFORE Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // 3. ROUTING
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
