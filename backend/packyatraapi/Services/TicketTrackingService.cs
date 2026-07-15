using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models.DTOs;
using MoversAndPackerApi.Services.Interfaces;
using System.Data;

namespace MoversAndPackerApi.Services
{
    public class TicketTrackingService : ITicketTrackingService
    {
        private readonly ApplicationDbContext _context;

        public TicketTrackingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public TicketTrackingDto TrackTicket(string phoneNumber, string ticketNo)
        {
            var response = new TicketTrackingDto();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.usp_TrackTicketByPhoneAndTicketNo";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PhoneNumber", phoneNumber));
                command.Parameters.Add(new SqlParameter("@TicketNo", ticketNo));

                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    // -------- Result Set 1: Ticket Summary --------
                    if (reader.Read())
                    {
                        response.Ticket = new TicketSummaryDto
                        {
                            TicketID = reader.GetInt32(reader.GetOrdinal("TicketID")),
                            TicketNo = reader["TicketNo"].ToString(),
                            TicketStatus = reader["TicketStatus"].ToString(),
                            FromLocation = reader["FromLocation"].ToString(),
                            ToLocation = reader["ToLocation"].ToString(),
                            TicketCreatedAt = reader.GetDateTime(reader.GetOrdinal("TicketCreatedAt")),
                            TicketUpdatedAt = reader.GetDateTime(reader.GetOrdinal("TicketUpdatedAt")),
                            PickupDate = reader.GetDateTime(reader.GetOrdinal("PickupDate")),
                            BookingStatus = reader["BookingStatus"].ToString(),
                            Name = reader["Name"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString()
                        };
                    }

                    // -------- Result Set 2: Comments --------
                    reader.NextResult();
                    response.Comments = new List<TicketCommentDto>();

                    while (reader.Read())
                    {
                        response.Comments.Add(new TicketCommentDto
                        {
                            CommentId = reader.GetInt32(reader.GetOrdinal("CommentId")),
                            CommentType = reader["CommentType"].ToString(),
                            CommentText = reader["CommentText"].ToString(),
                            CreatedBy = 17,//reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                        });
                    }
                }

                _context.Database.CloseConnection();
            }

            return response;
        }
    }
}
