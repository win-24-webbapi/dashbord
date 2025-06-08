using Microsoft.EntityFrameworkCore;
using DashboardService.API.Data;
using DashboardService.API.Services;

namespace DashboardService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.WithOrigins(
                    "http://localhost:5173", // Local development
                    "https://ashy-bay-0b0f05003.6.azurestaticapps.net", // New Azure Static Web App
                    "https://dashboardshram-g7fha0caa6ambehn.swedencentral-01.azurewebsites.net" // DashboardService
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });

        // Add DbContext
        builder.Services.AddDbContext<DashboardServiceDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add HttpClient
        builder.Services.AddHttpClient();

        // Register services
        builder.Services.AddScoped<IDashboardService, DashboardService.API.Services.DashboardService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
} 