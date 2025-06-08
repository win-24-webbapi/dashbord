using System.ComponentModel.DataAnnotations;

namespace DashboardService.API.Models
{
    public class DashboardStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TotalEvents { get; set; }

        [Required]
        public int ActiveEvents { get; set; }

        [Required]
        public int TotalBookings { get; set; }

        [Required]
        public int TotalTickets { get; set; }

        [Required]
        public decimal TotalRevenue { get; set; }
    }
} 