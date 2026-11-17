using System;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using SportRental.Components;
using SportRental.Data;
using SportRental.Services;

namespace SportRental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=prokat.db"));
            builder.Services.AddScoped<AuthService>();

            var app = builder.Build();

            var factory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using (var db = factory.CreateDbContext())
            {
                DbSeeder.Seed(db);
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
