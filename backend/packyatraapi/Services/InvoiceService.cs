using Microsoft.Data.SqlClient;
using System.Data;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly string _connectionString;

        public InvoiceService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<InvoiceResponseDto> GenerateInvoiceAsync(string quotationNumber)
        {
            var response = new InvoiceResponseDto();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GenerateInvoice_ByQuotation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QuotationNumber", quotationNumber);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.Message = reader["Message"]?.ToString();
                            response.StatusCode = Convert.ToInt32(reader["StatusCode"]);

                            if (response.StatusCode == 1)
                            {
                                response.InvoiceID = reader["InvoiceID"] != DBNull.Value ? Convert.ToInt32(reader["InvoiceID"]) : null;
                                response.BookingID = reader["BookingID"] != DBNull.Value ? Convert.ToInt32(reader["BookingID"]) : null;
                                response.QuotationNumber = reader["QuotationNumber"]?.ToString();
                                response.InvoiceNumber = reader["InvoiceNumber"]?.ToString();
                                response.CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : null;
                            }
                        }
                    }
                }
            }

            return response;
        }
        public async Task<InvoiceDetailsResponse> GetInvoiceDetailsByNumberAsync(string invoiceNumber)
        {
            var response = new InvoiceDetailsResponse
            {
                Invoice = new InvoiceSummaryDto(),
                Items = new List<InvoiceItemDto>()
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_GetInvoiceDetails_ByInvoiceNumber", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Result 1
                        if (await reader.ReadAsync())
                        {
                            response.Invoice = new InvoiceSummaryDto
                            {
                                InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                                InvoiceNumber = Convert.ToString(reader["InvoiceNumber"]),
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
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

                        // Result 2
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Items.Add(new InvoiceItemDto
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
            }

            return response;
        }

    }
}
