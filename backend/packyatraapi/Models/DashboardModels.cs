namespace MoversAndPackerApi.Models
{
    public class DashboardStats
    {
        public int TotalBookings { get; set; }
        public int ActiveTickets { get; set; }
        public int CompletedMoves { get; set; }
        public decimal TotalRevenue { get; set; }
        public int NewUsersWithoutBooking { get; set; }
        public int ActiveSupervisors { get; set; }
        public int TotalSupervisors { get; set; }
        public int HighPriorityTickets { get; set; }
        public int MediumPriorityTickets { get; set; }
        public int LowPriorityTickets { get; set; }
        public decimal BookingTrend { get; set; }
        public decimal TicketTrend { get; set; }
        public decimal CompletedTrend { get; set; }
        public decimal RevenueTrend { get; set; }
        public decimal AvgResolutionDays { get; set; }
        public string CustomerSatisfaction { get; set; }
        public decimal AvgResponseHours { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class TicketStatusData
    {
        public string Status { get; set; }
        public int TicketCount { get; set; }
    }

    public class RecentBooking
    {
        public int BookingID { get; set; }
        public int? TicketID { get; set; }
        public string TicketNo { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public DateTime PickupDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BookingAmountPaid { get; set; }
        public string BookingStatus { get; set; }
        public string TicketStatus { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime? TicketCreated { get; set; }
        public int? AssignedSupervisorID { get; set; }
        public string FormattedPickupDate { get; set; }
        public string FormattedTicketDate { get; set; }
        public int DaysSincePickup { get; set; }
        public string PaymentStatus { get; set; }
        public string QuotationNumber { get; set; }
        public string ticketdistribution {  get; set; }
    }

    public class MonthlyRevenueData
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
        public decimal AvgBookingValue { get; set; }
    }

    public class DashboardResponse
    {
        public DashboardStats Stats { get; set; }
        public ChartData TicketStatusChart { get; set; }
        public List<RecentBooking> RecentBookings { get; set; }
        public List<MonthlyRevenueData> MonthlyRevenue { get; set; }
    }

    public class ChartData
    {
        public List<string> Labels { get; set; }
        public List<ChartDataset> Datasets { get; set; }
    }

    public class ChartDataset
    {
        public string Label { get; set; }
        public List<int> Data { get; set; }
        public List<string> BackgroundColor { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}