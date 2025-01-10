using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using BulkyBook.DataAccess.DbInitializer;

namespace BulkyBookWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            var ConnectionString = builder.Configuration.GetConnectionString("cs");
            builder.Services.AddDbContext<ApplicationDbContext>( option =>option.UseSqlServer(ConnectionString));

            builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(option => {

                option.LoginPath = $"/Identity/Account/Login";
                option.LogoutPath = $"/Identity/Account/Logout";
                option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            builder.Services.AddRazorPages();


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddScoped<IUnitOfWork, UniteOfWork>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "1539959653343984";
                options.AppSecret = "d2e87661329438bccd93a760b720d112";
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession( options =>
            {
                options.IdleTimeout =TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;  
            });








            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            SeedDatabase();
                app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();


            void SeedDatabase()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbinitlizer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbinitlizer.Initialize();
                }
                    
            }
        }
    }
}
