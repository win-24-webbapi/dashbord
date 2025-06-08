using Microsoft.AspNetCore.Mvc;
using DashboardService.API.Models;
using DashboardService.API.Services;

namespace DashboardService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStats>> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpPut("stats")]
        public async Task<IActionResult> UpdateDashboardStats(DashboardStats stats)
        {
            await _dashboardService.UpdateDashboardStatsAsync(stats);
            return NoContent();
        }
    }
} 