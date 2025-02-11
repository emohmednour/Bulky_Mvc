using BulkyWebRazero_Temp.Data;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazero_Temp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connecting = builder.Configuration.GetConnectionString("cs");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connecting));

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.useAuthent
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
