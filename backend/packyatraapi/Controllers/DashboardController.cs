using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using MoversAndPackerApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/Dashboard/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllDashboardData()
        {
            try
            {
                var data = await _dashboardService.GetAllDashboardDataAsync();

                return Ok(new ApiResponse<DashboardResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Dashboard data fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<DashboardResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error fetching dashboard data: {ex.Message}"
                });
            }
        }

        // GET: api/Dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();

                return Ok(new ApiResponse<DashboardStats>
                {
                    Success = true,
                    Data = stats,
                    Message = "Dashboard stats fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<DashboardStats>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error fetching dashboard stats: {ex.Message}"
                });
            }
        }

        // GET: api/Dashboard/ticket-status
        [HttpGet("ticket-status")]
        public async Task<IActionResult> GetTicketStatusData()
        {
            try
            {
                var statusData = await _dashboardService.GetTicketStatusDataAsync();
                var chartData = new ChartData
                {
                    Labels = statusData.Select(t => t.Status).ToList(),
                    Datasets = new List<ChartDataset>
                    {
                        new ChartDataset
                        {
                            Label = "Ticket Count",
                            Data = statusData.Select(t => t.TicketCount).ToList(),
                            BackgroundColor = statusData.Select(t => GetStatusColor(t.Status)).ToList()
                        }
                    }
                };

                return Ok(new ApiResponse<ChartData>
                {
                    Success = true,
                    Data = chartData,
                    Message = "Ticket status data fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ChartData>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error fetching ticket status data: {ex.Message}"
                });
            }
        }

        // GET: api/Dashboard/recent-bookings
        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings()
        {
            try
            {
                var bookings = await _dashboardService.GetRecentBookingsAsync();

                return Ok(new ApiResponse<List<RecentBooking>>
                {
                    Success = true,
                    Data = bookings,
                    Message = "Recent bookings fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<RecentBooking>>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error fetching recent bookings: {ex.Message}"
                });
            }
        }

        // POST: api/Dashboard/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshDashboard()
        {
            try
            {
                // You can add any refresh logic here (cache clearing, etc.)
                await Task.Delay(100); // Simulate async operation

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    Message = "Dashboard refreshed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error refreshing dashboard: {ex.Message}"
                });
            }
        }

        // Helper method for status colors
        private string GetStatusColor(string status)
        {
            return status switch
            {
                "Pending" => "#ffc107",
                "In Progress" => "#17a2b8",
                "Assigned" => "#007bff",
                "Resolved" => "#28a745",
                "Closed" => "#28a745",
                "Cancelled" => "#dc3545",
                "Last Month" => "#6c757d",
                _ => "#6c757d"
            };
        }

        // GET: api/Dashboard/quotation/QTN12345
        [HttpGet("quotation/{quotationNumber}")]
        public async Task<IActionResult> GetQuotationDetails(string quotationNumber)
        {
            try
            {
                var data = await _dashboardService.GetQuotationDetailsByNumberAsync(quotationNumber);

                return Ok(new ApiResponse<QuotationDetailsResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Quotation details fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<QuotationDetailsResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error fetching quotation details: {ex.Message}"
                });
            }
        }

    }
}