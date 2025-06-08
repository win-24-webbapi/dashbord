using DashboardService.API.Models;
using DashboardService.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace DashboardService.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly DashboardServiceDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DashboardService(DashboardServiceDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            try
            {
                // Get events from EventService
                var eventServiceUrl = _configuration["Services:EventService"];
                Console.WriteLine($"Fetching events from: {eventServiceUrl}/api/event");
                var events = await _httpClient.GetFromJsonAsync<List<EventDto>>($"{eventServiceUrl}/api/event");

                if (events == null)
                {
                    Console.WriteLine("No events returned from EventService");
                    return new DashboardStats
                    {
                        TotalEvents = 0,
                        TotalBookings = 0,
                        TotalTickets = 0,
                        TotalRevenue = 0
                    };
                }

                Console.WriteLine($"Received {events.Count} events from EventService");
                foreach (var evt in events)
                {
                    Console.WriteLine($"Event: {evt.Title}, Price: {evt.Price}");
                }

                var now = DateTime.UtcNow;
                var stats = new DashboardStats
                {
                    TotalEvents = events.Count,
                    TotalTickets = events.Sum(e => e.MaxParticipants ?? 0),
                    TotalRevenue = events.Sum(e => e.Price)
                };

                Console.WriteLine($"Calculated stats: TotalEvents={stats.TotalEvents}, TotalRevenue={stats.TotalRevenue}");

                // Update stats in database
                var existingStats = await _context.DashboardStats.FirstOrDefaultAsync();
                if (existingStats != null)
                {
                    existingStats.TotalEvents = stats.TotalEvents;
                    existingStats.TotalTickets = stats.TotalTickets;
                    existingStats.TotalRevenue = stats.TotalRevenue;
                }
                else
                {
                    _context.DashboardStats.Add(stats);
                }
                await _context.SaveChangesAsync();

                return stats;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error calculating dashboard stats: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                
                // Return cached stats if available
                var cachedStats = await _context.DashboardStats.FirstOrDefaultAsync();
                return cachedStats ?? new DashboardStats
                {
                    TotalEvents = 0,
                    TotalBookings = 0,
                    TotalTickets = 0,
                    TotalRevenue = 0
                };
            }
        }

        public async Task UpdateDashboardStatsAsync(DashboardStats stats)
        {
            var existingStats = await _context.DashboardStats.FirstOrDefaultAsync();
            if (existingStats != null)
            {
                existingStats.TotalEvents = stats.TotalEvents;
                existingStats.TotalTickets = stats.TotalTickets;
                existingStats.TotalRevenue = stats.TotalRevenue;
            }
            else
            {
                _context.DashboardStats.Add(stats);
            }
            await _context.SaveChangesAsync();
        }
    }

    public class EventDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime StartDate { get; set; }
        public required string Location { get; set; }
        public int? MaxParticipants { get; set; }
        public int? CurrentParticipants { get; set; }
        public decimal Price { get; set; }
    }
} 