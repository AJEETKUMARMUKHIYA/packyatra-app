
using System.Data;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace MoversAndPackerApi.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly string _connectionString;

        public VehicleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ✅ Get All Vehicles
        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            var list = new List<Vehicle>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Vehicles ORDER BY CreatedAt DESC";
                var command = new SqlCommand(query, connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Vehicle
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VehicleNumber = Convert.ToString(reader["VehicleNumber"]),
                            VehicleType = Convert.ToString(reader["VehicleType"]),
                            DriverName = Convert.ToString(reader["DriverName"]),
                            DriverPhone = Convert.ToString(reader["DriverPhone"]),
                            Capacity = reader["Capacity"] != DBNull.Value ? Convert.ToInt32(reader["Capacity"]) : 0,
                            Status = Convert.ToString(reader["Status"])
                        });
                    }
                }
            }

            return list;
        }

        // ✅ Get Available Vehicles
        public async Task<List<Vehicle>> GetAvailableVehiclesAsync()
        {
            var list = new List<Vehicle>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Vehicles WHERE Status='Available'", connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Vehicle
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VehicleNumber = Convert.ToString(reader["VehicleNumber"]),
                            DriverName = Convert.ToString(reader["DriverName"]),
                            VehicleType = Convert.ToString(reader["VehicleType"]),
                            Status = Convert.ToString(reader["Status"])
                        });
                    }
                }
            }

            return list;
        }

        // ✅ Get By Id
        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Vehicles WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Vehicle
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VehicleNumber = Convert.ToString(reader["VehicleNumber"]),
                            VehicleType = Convert.ToString(reader["VehicleType"]),
                            DriverName = Convert.ToString(reader["DriverName"]),
                            DriverPhone = Convert.ToString(reader["DriverPhone"]),
                            Capacity = Convert.ToInt32(reader["Capacity"]),
                            Status = Convert.ToString(reader["Status"])
                        };
                    }
                }
            }

            return null;
        }

        // ✅ Create Vehicle
        public async Task<Vehicle> CreateVehicleAsync(VehicleCreateDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Vehicles 
                    (VehicleNumber, VehicleType, DriverName, DriverPhone, Capacity)
                    OUTPUT INSERTED.Id
                    VALUES (@VehicleNumber, @VehicleType, @DriverName, @DriverPhone, @Capacity)", connection);

                command.Parameters.AddWithValue("@VehicleNumber", dto.VehicleNumber);
                command.Parameters.AddWithValue("@VehicleType", dto.VehicleType);
                command.Parameters.AddWithValue("@DriverName", dto.DriverName);
                command.Parameters.AddWithValue("@DriverPhone", dto.DriverPhone);
                command.Parameters.AddWithValue("@Capacity", dto.Capacity);

                await connection.OpenAsync();

                var id = (int)await command.ExecuteScalarAsync();

                return new Vehicle
                {
                    Id = id,
                    VehicleNumber = dto.VehicleNumber,
                    VehicleType = dto.VehicleType,
                    DriverName = dto.DriverName,
                    DriverPhone = dto.DriverPhone,
                    Capacity = dto.Capacity,
                    Status = "Available"
                };
            }
        }

        // ✅ Update Vehicle
        public async Task<Vehicle> UpdateVehicleAsync(int id, VehicleUpdateDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    UPDATE Vehicles SET
                        VehicleType=@VehicleType,
                        DriverName=@DriverName,
                        DriverPhone=@DriverPhone,
                        Capacity=@Capacity,
                        Status=@Status,
                        UpdatedAt=GETDATE()
                    WHERE Id=@Id", connection);

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@VehicleType", dto.VehicleType);
                command.Parameters.AddWithValue("@DriverName", dto.DriverName);
                command.Parameters.AddWithValue("@DriverPhone", dto.DriverPhone);
                command.Parameters.AddWithValue("@Capacity", dto.Capacity);
                command.Parameters.AddWithValue("@Status", dto.Status);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return await GetVehicleByIdAsync(id);
        }

        // ✅ Delete Vehicle
        public async Task<bool> DeleteVehicleAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Vehicles WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                var rows = await command.ExecuteNonQueryAsync();

                return rows > 0;
            }
        }

        public async Task<bool> AssignVehicleAsync(AssignVehicleDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var transaction = connection.BeginTransaction();

                try
                {
                    // Check vehicle exists and status
                    var checkCmd = new SqlCommand(@"
                SELECT Status
                FROM Vehicles
                WHERE Id = @Id",
                        connection,
                        transaction);

                    checkCmd.Parameters.AddWithValue("@Id", dto.VehicleId);

                    var statusObj = await checkCmd.ExecuteScalarAsync();

                    if (statusObj == null)
                        throw new Exception("Vehicle not found");

                    var status = statusObj.ToString();

                    //if (status != "Available")
                    //    throw new Exception("Vehicle not available");

                    // Check if booking already has vehicle
                    var checkBookingCmd = new SqlCommand(@"
                SELECT VehicleId
                FROM BookingVehicles
                WHERE BookingId = @BookingId",
                        connection,
                        transaction);

                    checkBookingCmd.Parameters.AddWithValue(
                        "@BookingId",
                        dto.BookingId);

                    var existingVehicleObj =
                        await checkBookingCmd.ExecuteScalarAsync();

                    // If already assigned vehicle exists
                    if (existingVehicleObj != null &&
                        existingVehicleObj != DBNull.Value)
                    {
                        int oldVehicleId =
                            Convert.ToInt32(existingVehicleObj);

                        // Make old vehicle available
                        var oldVehicleUpdateCmd = new SqlCommand(@"
                    UPDATE Vehicles
                    SET Status = 'Available',
                        UpdatedAt = GETDATE()
                    WHERE Id = @OldVehicleId",
                            connection,
                            transaction);

                        oldVehicleUpdateCmd.Parameters.AddWithValue(
                            "@OldVehicleId",
                            oldVehicleId);

                        await oldVehicleUpdateCmd.ExecuteNonQueryAsync();

                        // Update booking vehicle mapping
                        var updateBookingVehicleCmd = new SqlCommand(@"
                    UPDATE BookingVehicles
                    SET VehicleId = @VehicleId
                    WHERE BookingId = @BookingId",
                            connection,
                            transaction);

                        updateBookingVehicleCmd.Parameters.AddWithValue(
                            "@BookingId",
                            dto.BookingId);

                        updateBookingVehicleCmd.Parameters.AddWithValue(
                            "@VehicleId",
                            dto.VehicleId);

                        await updateBookingVehicleCmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        // Insert new mapping
                        var insertCmd = new SqlCommand(@"
                    INSERT INTO BookingVehicles
                    (BookingId, VehicleId)
                    VALUES
                    (@BookingId, @VehicleId)",
                            connection,
                            transaction);

                        insertCmd.Parameters.AddWithValue(
                            "@BookingId",
                            dto.BookingId);

                        insertCmd.Parameters.AddWithValue(
                            "@VehicleId",
                            dto.VehicleId);

                        await insertCmd.ExecuteNonQueryAsync();
                    }

                    // Make selected vehicle busy
                    var updateVehicleCmd = new SqlCommand(@"
                UPDATE Vehicles
                SET Status = 'Busy',
                    UpdatedAt = GETDATE()
                WHERE Id = @VehicleId",
                        connection,
                        transaction);

                    updateVehicleCmd.Parameters.AddWithValue(
                        "@VehicleId",
                        dto.VehicleId);

                    await updateVehicleCmd.ExecuteNonQueryAsync();

                    // Update ticket vehicle id
                    var updateTicketCmd = new SqlCommand(@"
                UPDATE Tickets
                SET VehicleId = @VehicleId
                WHERE TicketID = @TicketID",
                        connection,
                        transaction);

                    updateTicketCmd.Parameters.AddWithValue(
                        "@VehicleId",
                        dto.VehicleId);

                    updateTicketCmd.Parameters.AddWithValue(
                        "@TicketID",
                        dto.BookingId);

                    await updateTicketCmd.ExecuteNonQueryAsync();

                    transaction.Commit();

                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> UnassignVehicleAsync(
            int bookingId,
            int vehicleId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var transaction = connection.BeginTransaction();

                try
                {
                    // Remove mapping
                    var deleteCmd = new SqlCommand(@"
                DELETE FROM BookingVehicles
                WHERE BookingId = @BookingId
                AND VehicleId = @VehicleId",
                        connection,
                        transaction);

                    deleteCmd.Parameters.AddWithValue(
                        "@BookingId",
                        bookingId);

                    deleteCmd.Parameters.AddWithValue(
                        "@VehicleId",
                        vehicleId);

                    var rows =
                        await deleteCmd.ExecuteNonQueryAsync();

                    // Make vehicle available
                    var updateVehicleCmd = new SqlCommand(@"
                UPDATE Vehicles
                SET Status = 'Available',
                    UpdatedAt = GETDATE()
                WHERE Id = @VehicleId",
                        connection,
                        transaction);

                    updateVehicleCmd.Parameters.AddWithValue(
                        "@VehicleId",
                        vehicleId);

                    await updateVehicleCmd.ExecuteNonQueryAsync();

                    // Remove vehicle from ticket
                    var updateTicketCmd = new SqlCommand(@"
                UPDATE Tickets
                SET VehicleId = NULL
                WHERE TicketID = @TicketID",
                        connection,
                        transaction);

                    updateTicketCmd.Parameters.AddWithValue(
                        "@TicketID",
                        bookingId);

                    await updateTicketCmd.ExecuteNonQueryAsync();

                    transaction.Commit();

                    return rows > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // ✅ Assign Vehicle
        //public async Task<bool> AssignVehicleAsync(AssignVehicleDto dto)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var transaction = connection.BeginTransaction();

        //        try
        //        {
        //            // Check status
        //            var checkCmd = new SqlCommand(
        //                "SELECT Status FROM Vehicles WHERE Id=@Id",
        //                connection, transaction);

        //            checkCmd.Parameters.AddWithValue("@Id", dto.VehicleId);

        //            var status = (string)await checkCmd.ExecuteScalarAsync();

        //            if (status != "Available")
        //                throw new Exception("Vehicle not available");

        //            // Insert mapping
        //            var insertCmd = new SqlCommand(@"
        //                INSERT INTO BookingVehicles (BookingId, VehicleId)
        //                VALUES (@BookingId, @VehicleId)",
        //                connection, transaction);

        //            insertCmd.Parameters.AddWithValue("@BookingId", dto.BookingId);
        //            insertCmd.Parameters.AddWithValue("@VehicleId", dto.VehicleId);

        //            await insertCmd.ExecuteNonQueryAsync();

        //            // Update vehicle status
        //            var updateCmd = new SqlCommand(@"
        //                UPDATE Vehicles SET Status='Busy', UpdatedAt=GETDATE()
        //                WHERE Id=@Id",
        //                connection, transaction);

        //            updateCmd.Parameters.AddWithValue("@Id", dto.VehicleId);

        //            await updateCmd.ExecuteNonQueryAsync();

        //            transaction.Commit();
        //            return true;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}
        //    public async Task<bool> AssignVehicleAsync(AssignVehicleDto dto)
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            var transaction = connection.BeginTransaction();

        //            try
        //            {
        //                // Check vehicle status
        //                var checkCmd = new SqlCommand(
        //                    "SELECT Status FROM Vehicles WHERE Id=@Id",
        //                    connection,
        //                    transaction);

        //                checkCmd.Parameters.AddWithValue("@Id", dto.VehicleId);

        //                var status = (string)await checkCmd.ExecuteScalarAsync();

        //                if (status != "Available")
        //                    throw new Exception("Vehicle not available");

        //                // Insert mapping
        //                // Check existing mapping
        //                var checkBookingCmd = new SqlCommand(@"
        //SELECT COUNT(1)
        //FROM BookingVehicles
        //WHERE BookingId = @BookingId",
        //                    connection,
        //                    transaction);

        //                checkBookingCmd.Parameters.AddWithValue("@BookingId", dto.BookingId);

        //                var exists = Convert.ToInt32(await checkBookingCmd.ExecuteScalarAsync());

        //                if (exists > 0)
        //                {
        //                    // Update existing vehicle mapping
        //                    var updateBookingVehicleCmd = new SqlCommand(@"
        //    UPDATE BookingVehicles
        //    SET VehicleId = @VehicleId
        //    WHERE BookingId = @BookingId",
        //                        connection,
        //                        transaction);

        //                    updateBookingVehicleCmd.Parameters.AddWithValue("@BookingId", dto.BookingId);
        //                    updateBookingVehicleCmd.Parameters.AddWithValue("@VehicleId", dto.VehicleId);

        //                    await updateBookingVehicleCmd.ExecuteNonQueryAsync();
        //                }
        //                else
        //                {
        //                    // Insert new mapping
        //                    var insertCmd = new SqlCommand(@"
        //    INSERT INTO BookingVehicles (BookingId, VehicleId)
        //    VALUES (@BookingId, @VehicleId)",
        //                        connection,
        //                        transaction);

        //                    insertCmd.Parameters.AddWithValue("@BookingId", dto.BookingId);
        //                    insertCmd.Parameters.AddWithValue("@VehicleId", dto.VehicleId);

        //                    await insertCmd.ExecuteNonQueryAsync();
        //                }

        //                // Update vehicle status
        //                var updateVehicleCmd = new SqlCommand(@"
        //            UPDATE Vehicles
        //            SET Status='Busy',
        //                UpdatedAt=GETDATE()
        //            WHERE Id=@Id",
        //                    connection,
        //                    transaction);

        //                updateVehicleCmd.Parameters.AddWithValue("@Id", dto.VehicleId);

        //                await updateVehicleCmd.ExecuteNonQueryAsync();

        //                // Update ticket vehicle id
        //                var updateTicketCmd = new SqlCommand(@"
        //            UPDATE Tickets
        //            SET VehicleId = @VehicleId
        //            WHERE TicketID = @TicketID",
        //                    connection,
        //                    transaction);

        //                updateTicketCmd.Parameters.AddWithValue("@VehicleId", dto.VehicleId);
        //                updateTicketCmd.Parameters.AddWithValue("@TicketID", dto.BookingId);

        //                await updateTicketCmd.ExecuteNonQueryAsync();

        //                transaction.Commit();

        //                return true;
        //            }
        //            catch
        //            {
        //                transaction.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //public async Task<bool> UnassignVehicleAsync(int bookingId, int vehicleId)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var transaction = connection.BeginTransaction();

        //        try
        //        {
        //            // Remove mapping
        //            var deleteCmd = new SqlCommand(@"
        //        DELETE FROM BookingVehicles
        //        WHERE BookingId = @BookingId
        //        AND VehicleId = @VehicleId",
        //                connection,
        //                transaction);

        //            deleteCmd.Parameters.AddWithValue("@BookingId", bookingId);
        //            deleteCmd.Parameters.AddWithValue("@VehicleId", vehicleId);

        //            var rows = await deleteCmd.ExecuteNonQueryAsync();

        //            // Update vehicle status
        //            var updateVehicleCmd = new SqlCommand(@"
        //        UPDATE Vehicles
        //        SET Status = 'Available',
        //            UpdatedAt = GETDATE()
        //        WHERE Id = @VehicleId",
        //                connection,
        //                transaction);

        //            updateVehicleCmd.Parameters.AddWithValue("@VehicleId", vehicleId);

        //            await updateVehicleCmd.ExecuteNonQueryAsync();

        //            // Remove vehicle from ticket
        //            var updateTicketCmd = new SqlCommand(@"
        //        UPDATE Tickets
        //        SET VehicleId = NULL
        //        WHERE TicketID = @TicketID",
        //                connection,
        //                transaction);

        //            updateTicketCmd.Parameters.AddWithValue("@TicketID", bookingId);

        //            await updateTicketCmd.ExecuteNonQueryAsync();

        //            transaction.Commit();

        //            return rows > 0;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}
        //public async Task<bool> UnassignVehicleAsync(int bookingId, int vehicleId)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var transaction = connection.BeginTransaction();

        //        try
        //        {
        //            // Remove vehicle mapping from booking
        //            var deleteCmd = new SqlCommand(@"
        //        DELETE FROM BookingVehicles
        //        WHERE BookingId = @BookingId
        //        AND VehicleId = @VehicleId",
        //                connection,
        //                transaction);

        //            deleteCmd.Parameters.AddWithValue("@BookingId", bookingId);
        //            deleteCmd.Parameters.AddWithValue("@VehicleId", vehicleId);

        //            var rows = await deleteCmd.ExecuteNonQueryAsync();

        //            // Update vehicle status back to Available
        //            var updateCmd = new SqlCommand(@"
        //        UPDATE Vehicles
        //        SET Status = 'Available',
        //            UpdatedAt = GETDATE()
        //        WHERE Id = @VehicleId",
        //                connection,
        //                transaction);

        //            updateCmd.Parameters.AddWithValue("@VehicleId", vehicleId);

        //            await updateCmd.ExecuteNonQueryAsync();

        //            transaction.Commit();

        //            return rows > 0;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}
    }
}