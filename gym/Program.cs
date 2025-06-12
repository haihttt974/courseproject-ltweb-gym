using gym.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DinkToPdf; // Install-Package DinkToPdf
using DinkToPdf.Contracts; //Install - Package DinkToPdf.Contracts

namespace gym
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<GymContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";                // Chưa đăng nhập
                    options.AccessDeniedPath = "/Error/404";       // Đã login nhưng sai role
                });

            //builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            //builder.Services.AddSingleton<EmailService>();
            //builder.Services.AddHostedService<ExpiredPackageChecker>();
            //builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Môi trường DEV: hiển thị lỗi chi tiết
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Môi trường PROD: chuyển hướng lỗi đến controller
                app.UseExceptionHandler("/Error/Error500");
                app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Cho 404, 403, ...
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
