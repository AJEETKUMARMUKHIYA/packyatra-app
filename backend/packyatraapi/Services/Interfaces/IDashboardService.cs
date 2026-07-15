using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<List<TicketStatusData>> GetTicketStatusDataAsync();
        Task<List<RecentBooking>> GetRecentBookingsAsync();
        Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync();
        Task<DashboardResponse> GetAllDashboardDataAsync();
        Task<QuotationDetailsResponse> GetQuotationDetailsByNumberAsync(string quotationNumber);
    }
}
