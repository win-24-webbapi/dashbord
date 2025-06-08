using DashboardService.API.Models;

namespace DashboardService.API.Services
{
    public interface IDashboardService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task UpdateDashboardStatsAsync(DashboardStats stats);
    }
} 