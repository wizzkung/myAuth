using Microsoft.AspNetCore.Authentication.Cookies;
using myAuth.Abstraction;
using myAuth.Service;

namespace myAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication
                (CookieAuthenticationDefaults.AuthenticationScheme)
                    //.AddCookie(o => o.LoginPath = new PathString("/Account/Login"));
                    .AddCookie(options =>
                    {
                        options.Cookie.Name = "MyIdentity";
                        //options.ExpireTimeSpan = TimeSpan.FromHours(8);
                        options.SlidingExpiration = true;
                        options.Cookie.MaxAge = TimeSpan.FromHours(8);
                        options.LoginPath = new PathString("/Account/Login");
                    });


            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IService, AuthService>();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}