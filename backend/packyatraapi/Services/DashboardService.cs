using Microsoft.Data.SqlClient;
using System.Data;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Services
{
    //public interface IDashboardService
    //{
    //    Task<DashboardStats> GetDashboardStatsAsync();
    //    Task<List<TicketStatusData>> GetTicketStatusDataAsync();
    //    Task<List<RecentBooking>> GetRecentBookingsAsync(int topCount = 8);
    //    Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync();
    //    Task<DashboardResponse> GetAllDashboardDataAsync();
    //    Task<QuotationDetailsResponse> GetQuotationDetailsByNumberAsync(string quotationNumber);
    //}

    public class DashboardService : IDashboardService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DashboardService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetDashboardStats", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            stats = new DashboardStats
                            {
                                TotalBookings = reader["TotalBookings"] != DBNull.Value ? Convert.ToInt32(reader["TotalBookings"]) : 0,
                                ActiveTickets = reader["ActiveTickets"] != DBNull.Value ? Convert.ToInt32(reader["ActiveTickets"]) : 0,
                                CompletedMoves = reader["CompletedMoves"] != DBNull.Value ? Convert.ToInt32(reader["CompletedMoves"]) : 0,
                                TotalRevenue = reader["TotalRevenue"] != DBNull.Value ? Convert.ToDecimal(reader["TotalRevenue"]) : 0,
                                NewUsersWithoutBooking = reader["NewUsersWithoutBooking"] != DBNull.Value ? Convert.ToInt32(reader["NewUsersWithoutBooking"]) : 0,
                                ActiveSupervisors = reader["ActiveSupervisors"] != DBNull.Value ? Convert.ToInt32(reader["ActiveSupervisors"]) : 0,
                                TotalSupervisors = reader["TotalSupervisors"] != DBNull.Value ? Convert.ToInt32(reader["TotalSupervisors"]) : 1,
                                HighPriorityTickets = reader["HighPriorityTickets"] != DBNull.Value ? Convert.ToInt32(reader["HighPriorityTickets"]) : 0,
                                MediumPriorityTickets = reader["MediumPriorityTickets"] != DBNull.Value ? Convert.ToInt32(reader["MediumPriorityTickets"]) : 0,
                                LowPriorityTickets = reader["LowPriorityTickets"] != DBNull.Value ? Convert.ToInt32(reader["LowPriorityTickets"]) : 0,
                                BookingTrend = reader["BookingTrend"] != DBNull.Value ? Convert.ToDecimal(reader["BookingTrend"]) : 0,
                                TicketTrend = reader["TicketTrend"] != DBNull.Value ? Convert.ToDecimal(reader["TicketTrend"]) : 0,
                                CompletedTrend = reader["CompletedTrend"] != DBNull.Value ? Convert.ToDecimal(reader["CompletedTrend"]) : 0,
                                RevenueTrend = reader["RevenueTrend"] != DBNull.Value ? Convert.ToDecimal(reader["RevenueTrend"]) : 0,
                                AvgResolutionDays = reader["AvgResolutionDays"] != DBNull.Value ? Convert.ToDecimal(reader["AvgResolutionDays"]) : 2.4m,
                                CustomerSatisfaction = reader["CustomerSatisfaction"] != DBNull.Value ? Convert.ToString(reader["CustomerSatisfaction"]) : "94%",
                                AvgResponseHours = reader["AvgResponseHours"] != DBNull.Value ? Convert.ToDecimal(reader["AvgResponseHours"]) : 2.4m,
                                LastUpdated = reader["LastUpdated"] != DBNull.Value ? Convert.ToDateTime(reader["LastUpdated"]) : DateTime.Now
                            };
                        }
                    }
                }
            }

            return stats;
        }

        public async Task<List<TicketStatusData>> GetTicketStatusDataAsync()
        {
            var statusData = new List<TicketStatusData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetTicketStatusData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var data = new TicketStatusData
                            {
                                Status = reader["Status"] != DBNull.Value ? Convert.ToString(reader["Status"]) : "Unknown",
                                TicketCount = reader["TicketCount"] != DBNull.Value ? Convert.ToInt32(reader["TicketCount"]) : 0
                            };
                            statusData.Add(data);
                        }
                    }
                }
            }

            return statusData;
        }

        public async Task<List<RecentBooking>> GetRecentBookingsAsync()
        {
            var bookings = new List<RecentBooking>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetRecentBookingsWithDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@TopCount", topCount);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var booking = new RecentBooking
                            {
                                BookingID = reader["BookingID"] != DBNull.Value ? Convert.ToInt32(reader["BookingID"]) : 0,
                                TicketID = reader["TicketID"] != DBNull.Value ? Convert.ToInt32(reader["TicketID"]) : (int?)null,
                                TicketNo = reader["TicketNo"] != DBNull.Value ? Convert.ToString(reader["TicketNo"]) : "N/A",
                                UserName = reader["UserName"] != DBNull.Value ? Convert.ToString(reader["UserName"]) : "",
                                UserEmail = reader["UserEmail"] != DBNull.Value ? Convert.ToString(reader["UserEmail"]) : "",
                                UserPhone = reader["UserPhone"] != DBNull.Value ? Convert.ToString(reader["UserPhone"]) : "",
                                PickupDate = reader["PickupDate"] != DBNull.Value ? Convert.ToDateTime(reader["PickupDate"]) : DateTime.MinValue,
                                TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0,
                                BookingAmountPaid = reader["BookingAmountPaid"] != DBNull.Value ? Convert.ToDecimal(reader["BookingAmountPaid"]) : 0,
                                BookingStatus = reader["BookingStatus"] != DBNull.Value ? Convert.ToString(reader["BookingStatus"]) : "Unknown",
                                TicketStatus = reader["TicketStatus"] != DBNull.Value ? Convert.ToString(reader["TicketStatus"]) : "No Ticket",
                                FromLocation = reader["FromAddress"] != DBNull.Value ? Convert.ToString(reader["FromAddress"]) : "Not specified",
                                ToLocation = reader["ToAddress"] != DBNull.Value ? Convert.ToString(reader["ToAddress"]) : "Not specified",
                                TicketCreated = reader["TicketCreated"] != DBNull.Value ? Convert.ToDateTime(reader["TicketCreated"]) : (DateTime?)null,
                                AssignedSupervisorID = reader["AssignedSupervisorID"] != DBNull.Value ? Convert.ToInt32(reader["AssignedSupervisorID"]) : (int?)null,
                                FormattedPickupDate = reader["FormattedPickupDate"] != DBNull.Value ? Convert.ToString(reader["FormattedPickupDate"]) : "",
                                FormattedTicketDate = reader["FormattedTicketDate"] != DBNull.Value ? Convert.ToString(reader["FormattedTicketDate"]) : "N/A",
                                DaysSincePickup = reader["DaysSincePickup"] != DBNull.Value ? Convert.ToInt32(reader["DaysSincePickup"]) : 0,
                                PaymentStatus = reader["PaymentStatus"] != DBNull.Value ? Convert.ToString(reader["PaymentStatus"]) : "Unknown",
                                QuotationNumber= reader["QuotationNumber"] !=DBNull.Value? Convert.ToString(reader["QuotationNumber"]):"Unknown",
                                ticketdistribution = reader["Ticket_distribution"] != DBNull.Value ? Convert.ToString(reader["Ticket_distribution"]) : "",

                            };
                            bookings.Add(booking);
                        }
                    }
                }
            }

            return bookings;
        }

        public async Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync()
        {
            var revenueData = new List<MonthlyRevenueData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetMonthlyRevenueChart", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var data = new MonthlyRevenueData
                            {
                                MonthNumber = reader["MonthNumber"] != DBNull.Value ? Convert.ToInt32(reader["MonthNumber"]) : 0,
                                MonthName = reader["MonthName"] != DBNull.Value ? Convert.ToString(reader["MonthName"]) : "",
                                Revenue = reader["Revenue"] != DBNull.Value ? Convert.ToDecimal(reader["Revenue"]) : 0,
                                BookingCount = reader["BookingCount"] != DBNull.Value ? Convert.ToInt32(reader["BookingCount"]) : 0,
                                AvgBookingValue = reader["AvgBookingValue"] != DBNull.Value ? Convert.ToDecimal(reader["AvgBookingValue"]) : 0
                            };
                            revenueData.Add(data);
                        }
                    }
                }
            }

            return revenueData;
        }

        public async Task<DashboardResponse> GetAllDashboardDataAsync()
        {
            var statsTask = GetDashboardStatsAsync();
            var ticketStatusTask = GetTicketStatusDataAsync();
            var bookingsTask = GetRecentBookingsAsync();
            var revenueTask = GetMonthlyRevenueAsync();

            await Task.WhenAll(statsTask, ticketStatusTask, bookingsTask, revenueTask);

            var chartData = new ChartData
            {
                Labels = ticketStatusTask.Result.Select(t => t.Status).ToList(),
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "Ticket Count",
                        Data = ticketStatusTask.Result.Select(t => t.TicketCount).ToList(),
                        BackgroundColor = ticketStatusTask.Result.Select(t => GetStatusColor(t.Status)).ToList()
                    }
                }
            };

            return new DashboardResponse
            {
                Stats = statsTask.Result,
                TicketStatusChart = chartData,
                RecentBookings = bookingsTask.Result,
                MonthlyRevenue = revenueTask.Result
            };
        }

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

        public async Task<QuotationDetailsResponse> GetQuotationDetailsByNumberAsync(string quotationNumber)

        {
            var response = new QuotationDetailsResponse
            {
                Quotation = new QuotationSummaryDto(),
                Items = new List<QuotationItemDto>()
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_GetBookingDetails_ByQuotation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QuotationNumber", quotationNumber);

                    await connection.OpenAsync();
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // ✅ First Result Set (Booking Details)
                            if (await reader.ReadAsync())
                            {
                                response.Quotation = new QuotationSummaryDto
                                {
                                    BookingID = Convert.ToInt32(reader["BookingID"]),
                                    QuotationNumber = Convert.ToString(reader["QuotationNumber"]),
                                    PickupDate = Convert.ToDateTime(reader["PickupDate"]),
                                    Status = Convert.ToString(reader["Status"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    BookingAmountPaid = Convert.ToDecimal(reader["BookingAmountPaid"]),
                                    TotalVolume = Convert.ToDecimal(reader["TotalVolume"]),
                                    MovingType = Convert.ToString(reader["MovingType"]),
                                    CustomerName = Convert.ToString(reader["CustomerName"]),
                                    PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                                    Email = Convert.ToString(reader["Email"]),
                                    FromAddress = Convert.ToString(reader["FromAddress"]),
                                    ToAddress = Convert.ToString(reader["ToAddress"]),
                                    SlotTime = Convert.ToString(reader["SlotTime"])
                                };
                            }

                            // ✅ Move to Second Result Set (Items)
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    response.Items.Add(new QuotationItemDto
                                    {
                                        BookingItemID = Convert.ToInt32(reader["BookingItemID"]),
                                        ItemID = Convert.ToInt32(reader["ItemID"]),
                                        ItemName = Convert.ToString(reader["ItemName"]),
                                        Category = Convert.ToString(reader["Category"]),
                                        Description = Convert.ToString(reader["Description"]),
                                        SizeCFT = Convert.ToDecimal(reader["SizeCFT"]),
                                        BookedQuantity = Convert.ToInt32(reader["BookedQuantity"])
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    
                }
            }

            return response;
        }

    }
}