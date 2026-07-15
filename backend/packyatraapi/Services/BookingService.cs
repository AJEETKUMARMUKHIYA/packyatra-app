using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateBookingAsync1(Booking booking)
        {
            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }
        public async Task<Booking> CreateBookingAsync(BookingDetails booking)
        {
            // Step 1: Create a new booking object without the BookingItemList
            var newBooking = new Booking
            {
                UserID = booking.UserID,
                SourceAddressID = booking.SourceAddressID,
                DestinationAddressID = booking.DestinationAddressID,
                PickupDate = booking.PickupDate,
                PickupTimeSlotID = booking.PickupTimeSlotID,
                Status = booking.Status,
                TotalAmount = booking.TotalAmount,
                BookingAmountPaid = booking.BookingAmountPaid,
                TotalVolume =booking.TotalVolume,
                Isquotation = false,
                Ticket_distribution = "Active"
            };

            // Step 2: Save the booking to generate BookingID
            _context.Booking.Add(newBooking);
            await _context.SaveChangesAsync();
            // Step 3: Assign QuotationNumber using generated BookingID
            newBooking.QuotationNumber = $"PACKY{newBooking.BookingID}";

            // Step 4: Update only QuotationNumber
            _context.Entry(newBooking).Property(x => x.QuotationNumber).IsModified = true;
            await _context.SaveChangesAsync();
            // Step 3: Now that we have the BookingID, insert BookingItems
            if (booking.BookingItemList != null && booking.BookingItemList.Any())
            {
                foreach (var item in booking.BookingItemList)
                {
                    var newBookingItem = new BookingItem
                    {
                        BookingID = newBooking.BookingID, // Assign newly created BookingID
                        ItemID = item.ItemID,
                        Quantity = item.Quantity,
                       // QuotationNumber = newBooking.QuotationNumber // ✅ correct
                    };

                    _context.BookingItem.Add(newBookingItem);
                }

                await _context.SaveChangesAsync(); // Save all booking items
            }

            return newBooking;
        }


        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _context.Booking.FindAsync(id);
        }

        //public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        //{
        //    return await _context.Booking.Where(b => b.UserID == userId).ToListAsync();
        //}

        //public async Task<bool> UpdateBookingAsync(Booking booking)
        //{
        //    _context.Booking.Update(booking);
        //    return await _context.SaveChangesAsync() > 0;
        //}
        public async Task<bool> UpdateBookingAsync(Booking bookingUpdate)
        {
            var existingBooking = await _context.Booking
                .FirstOrDefaultAsync(b => b.BookingID == bookingUpdate.BookingID);

            if (existingBooking == null)
                return false;

            // Update only the fields that should change
            existingBooking.BookingAmountPaid = bookingUpdate.BookingAmountPaid;
            existingBooking.Status = bookingUpdate.Status;
            existingBooking.TotalAmount = bookingUpdate.TotalAmount;
            existingBooking.QuotationNumber = bookingUpdate.QuotationNumber;
            existingBooking.Isquotation = bookingUpdate.Isquotation;
         
            // Don't update AddressID fields if they're not changing

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return false;

            _context.Booking.Remove(booking);
            return await _context.SaveChangesAsync() > 0;
        }
        //public async Task<List<dynamic>> GetBookingsByUserIdAsync(int userId)
        //{
        //    return await _context.Booking
        //        .Where(b => b.UserID == userId)
        //        .Select(b => new
        //        {
        //            BookingID = b.BookingID,
        //            UserID = b.UserID,
        //            SourceAddressID = b.SourceAddressID,
        //            DestinationAddressID = b.DestinationAddressID,
        //            PickupDate = b.PickupDate,
        //            PickupTimeSlotID = b.PickupTimeSlotID,
        //            Status = b.Status,
        //            TotalAmount = b.TotalAmount,
        //            BookingAmountPaid = b.BookingAmountPaid,

        //            BookingItems = b.BookingItems.Select(bi => new
        //            {
        //                BookingItemID = bi.BookingItemID,
        //                BookingID = bi.BookingID,
        //                ItemID = bi.ItemID,
        //                Quantity = bi.Quantity,
        //                ItemName = _context.InventoryItems
        //                    .Where(i => i.ItemID == bi.ItemID)
        //                    .Select(i => i.Name)
        //                    .FirstOrDefault() ?? "Unknown Item",
        //                ItemDescription = _context.InventoryItems
        //                    .Where(i => i.ItemID == bi.ItemID)
        //                    .Select(i => i.Description)
        //                    .FirstOrDefault(),
        //                ItemCategory = _context.InventoryItems
        //                    .Where(i => i.ItemID == bi.ItemID)
        //                    .Select(i => i.Category)
        //                    .FirstOrDefault(),
        //                ItemSize = _context.InventoryItems
        //                    .Where(i => i.ItemID == bi.ItemID)
        //                    .Select(i => i.SizeCFT)
        //                    .FirstOrDefault()
        //            }).ToList(),

        //            ItemsDescription = string.Join(", ", b.BookingItems.Select(bi =>
        //                _context.InventoryItems
        //                    .Where(i => i.ItemID == bi.ItemID)
        //                    .Select(i => $"{i.Name} x{bi.Quantity}")
        //                    .FirstOrDefault() ?? $"Item x{bi.Quantity}")),

        //            TotalItems = b.BookingItems.Sum(bi => bi.Quantity)
        //        })
        //        .OrderByDescending(b => b.PickupDate)
        //        .Cast<dynamic>() // Cast to dynamic
        //        .ToListAsync();
        //}
        public async Task<List<dynamic>> GetBookingsByUserIdAsync(int userId)
        {
            
            return await _context.Booking
                .Where(b => b.UserID == userId)
                .Select(b => new
                {
                    // Keep original property names but fix errors
                    BookingID = b.BookingID,
                    UserID = b.UserID,
                    SourceAddressID = b.SourceAddressID,
                    DestinationAddressID = b.DestinationAddressID,
                    PickupDate = b.PickupDate,
                    PickupTimeSlotID = b.PickupTimeSlotID,
                    Status = b.Status,
                    StatusText = b.Status == "pending" ? "Pending" :
             b.Status == "confirmed" ? "Confirmed" :
             b.Status == "in_transit" ? "In Transit" :
             b.Status == "completed" ? "Completed" :
             b.Status == "cancelled" ? "Cancelled" :
             b.Status,
            TotalAmount = b.TotalAmount,
                    BookingAmountPaid = b.BookingAmountPaid,
                    PaymentStatus = b.BookingAmountPaid >= b.TotalAmount ? "Paid" : "Partial",
                    CreatedDate = b.PickupDate,
                    UpdatedDate = b.PickupDate,

                    // Simplified address fields
                    From = _context.Address
                        .Where(a => a.AddressID == b.SourceAddressID)
                        .Select(a => a.FromAddress)
                        .FirstOrDefault() ?? "Address not available",

                    To = _context.Address
                        .Where(a => a.AddressID == b.DestinationAddressID)
                        .Select(a => a.ToAddress)
                        .FirstOrDefault() ?? "Address not available",

                    // Time slot
                    TimeSlot = _context.TimeSlots
                        .Where(t => t.TimeSlotID == b.PickupTimeSlotID)
                        .Select(t => t.TimeSlotName)
                        .FirstOrDefault() ?? "Time not set",

                    // Booking Items
                    BookingItems = b.BookingItems.Select(bi => new
                    {
                        BookingItemID = bi.BookingItemID,
                        BookingID = bi.BookingID,
                        ItemID = bi.ItemID,
                        Quantity = bi.Quantity,
                        ItemName = _context.InventoryItems
                            .Where(i => i.ItemID == bi.ItemID)
                            .Select(i => i.Name)
                            .FirstOrDefault() ?? "Unknown Item",
                        ItemDescription = _context.InventoryItems
                            .Where(i => i.ItemID == bi.ItemID)
                            .Select(i => i.Description)
                            .FirstOrDefault(),
                        ItemCategory = _context.InventoryItems
                            .Where(i => i.ItemID == bi.ItemID)
                            .Select(i => i.Category)
                            .FirstOrDefault(),
                        ItemSize = _context.InventoryItems
                            .Where(i => i.ItemID == bi.ItemID)
                            .Select(i => i.SizeCFT)
                            .FirstOrDefault()
                    }).ToList(),

                    // Combined items for display
                    Items = string.Join(", ", b.BookingItems.Select(bi =>
                        _context.InventoryItems
                            .Where(i => i.ItemID == bi.ItemID)
                            .Select(i => $"{i.Name} x{bi.Quantity}")
                            .FirstOrDefault() ?? $"Item x{bi.Quantity}")),

                    TotalItems = b.BookingItems.Sum(bi => bi.Quantity),

                    // Tracking
                    TrackingId = "BK-" + b.BookingID.ToString("D8"),

                    // Formatted date
                    Date = b.PickupDate.ToString("dd MMM yyyy"),

                    // For your frontend
                    Type = "Standard",
                    FromAddress = _context.Address
                        .Where(a => a.AddressID == b.SourceAddressID)
                        .Select(a => a.FromAddress)
                        .FirstOrDefault() ?? "Address not available",
                    ToAddress = _context.Address
                        .Where(a => a.AddressID == b.DestinationAddressID)
                        .Select(a => a.ToAddress)
                        .FirstOrDefault() ?? "Address not available"
                })
                .OrderByDescending(b => b.PickupDate)
                .ThenByDescending(b => b.BookingID)
                .Cast<dynamic>()
                .ToListAsync();
        }
        public async Task<Booking> GetBookingsByUserIdAsync1(int userId)
        {
            return await _context.Booking
         .Where(b => b.UserID == userId)
         .Select(b => new Booking
         {
             BookingID = b.BookingID,
             UserID = b.UserID,
             BookingItems = b.BookingItems.Select(bi => new BookingItem
             {
                 BookingItemID = bi.BookingItemID,
                 ItemID = bi.ItemID,
                 Quantity = bi.Quantity// Include other necessary properties
             }).ToList()
         })
         .FirstOrDefaultAsync(); // Fetch a single booking DTO or null
            //return await _context.Booking
            //    .Where(b => b.UserID == userId)
            //   // .Include(b => b.BookingItems) // Fetch associated booking items
            //                                  //.ThenInclude(i => i.Item)    // Fetch item details
            //    .FirstOrDefaultAsync();
            //       // Use FirstOrDefaultAsync to return a single booking or null
        }

        public async Task<object> UpdateBookingAndTicketAsync(UpdateBookingRequest request)
        {
            var bookingIdParam = new SqlParameter("@BookingID", request.BookingID);
            var amountParam = new SqlParameter("@TotalAmount",
                (object?)request.TotalAmount ?? DBNull.Value);
            var bookingStatusParam = new SqlParameter("@BookingStatus",
                (object?)request.BookingStatus ?? DBNull.Value);
            //var ticketStatusParam = new SqlParameter("@TicketStatus",
               // (object?)request.TicketStatus ?? DBNull.Value);

            var result = await _context.Database
                .ExecuteSqlRawAsync(
                    "EXEC UpdateBookingAndTicketFlexible @BookingID, @TotalAmount, @BookingStatus",
                    bookingIdParam,
                    amountParam,
                    bookingStatusParam
                  
                );

            return new
            {
                Success = true,
                Message = result > 0 ? "Updated Successfully" : "Updated Successfully"
            };
        }


        public async Task<List<dynamic>> GetBookingsBySupervisorIdAsync(int supervisorId)
        {
            return await _context.Booking
                .Join(_context.Tickets,
                    b => b.BookingID,
                    t => t.BookingID,
                    (b, t) => new { b, t })
                .Where(joined => joined.t.AssignedSupervisorID == supervisorId)
                .Select(joined => new
                {
                    BookingID = joined.b.BookingID,
                    TicketID = joined.t.TicketID,
                    TicketNo = joined.t.TicketNo,
                    UserID = joined.b.UserID,
                    Status = joined.t.Status, // Evaluates 'pending' or 'transit' inside React Native
                    TotalAmount = joined.b.TotalAmount,
                    VehicleId = joined.t.VehicleId,
                    CreatedAt = joined.t.CreatedAt,

                    // Pull Customer Details from UserAdmin table via UserID relationship map
                    FullName = _context.Users
                        .Where(u => u.UserID == joined.t.UserID)
                        .Select(u => u.Name)
                        .FirstOrDefault() ?? "Customer Name Not Found",

                    CustomerMobile = _context.Users
                        .Where(u => u.UserID == joined.t.UserID)
                        .Select(u => u.PhoneNumber)
                        .FirstOrDefault() ?? "",

                    // Match Addresses directly with your context maps
                    FromLocation = _context.Address
                        .Where(a => a.AddressID == joined.b.SourceAddressID)
                        .Select(a => a.FromAddress)
                        .FirstOrDefault() ?? "Address not available",

                    ToLocation = _context.Address
                        .Where(a => a.AddressID == joined.b.DestinationAddressID)
                        .Select(a => a.ToAddress)
                        .FirstOrDefault() ?? "Address not available",

                    // Parse drop-down parameters dynamically
                    TimeSlot = _context.TimeSlots
                        .Where(ts => ts.TimeSlotID == joined.b.PickupTimeSlotID)
                        .Select(ts => ts.TimeSlotName)
                        .FirstOrDefault() ?? "Time Slot Pending"
                })
                .OrderByDescending(x => x.CreatedAt)
                .Cast<dynamic>()
                .ToListAsync();
        }

        public async Task<List<dynamic>> GetBookingItemsByBookingIdAsync(int bookingId)
        {
            return await _context.BookingItem
                .Where(bi => bi.BookingID == bookingId)
                .Join(_context.InventoryItems,
                    bi => bi.ItemID,
                    ii => ii.ItemID,
                    (bi, ii) => new { bi, ii })
                .Select(joined => new
                {
                    BookingItemID = joined.bi.BookingItemID,
                    BookingID = joined.bi.BookingID,
                    ItemID = joined.bi.ItemID,
                    Quantity = joined.bi.Quantity,
                    ItemName = joined.ii.Name,
                    Category = joined.ii.Category,
                    SizeCFT = joined.ii.SizeCFT,
                    Description = joined.ii.Description,
                    // Automatically creates the uniform label display format for your frontend string matching rules
                    DisplayString = $"{joined.ii.Name} ({joined.bi.Quantity})"
                })
                .Cast<dynamic>()
                .ToListAsync();
        }

        //public async Task<List<dynamic>> GetBookingsBySupervisorIdAsync(int supervisorId)
        //{
        //    return await _context.Booking
        //        .Join(_context.Tickets,
        //            b => b.BookingID,
        //            t => t.BookingID,
        //            (b, t) => new { b, t })
        //        .Where(joined => joined.t.AssignedSupervisorID == supervisorId)
        //        .Select(joined => new
        //        {
        //            BookingID = joined.b.BookingID,
        //            TicketID = joined.t.TicketID,
        //            TicketNo = joined.t.TicketNo,
        //            UserID = joined.b.UserID,
        //            Status = joined.t.Status,
        //            TotalAmount = joined.b.TotalAmount,
        //            VehicleId = joined.t.VehicleId,
        //            CreatedAt = joined.t.CreatedAt,
        //            TotalVolume = joined.b.TotalVolume, // Added volume dynamically

        //            // Pull Customer Details from UserAdmin table via UserID relationship map
        //            FullName = _context.Users
        //                .Where(u => u.UserID == joined.t.UserID)
        //                .Select(u => u.Name)
        //                .FirstOrDefault() ?? "Customer Name Not Found",

        //            CustomerMobile = _context.Users
        //                .Where(u => u.UserID == joined.t.UserID)
        //                .Select(u => u.PhoneNumber)
        //                .FirstOrDefault() ?? "",

        //            // Match Addresses directly with your context maps
        //            FromLocation = _context.Address
        //                .Where(a => a.AddressID == joined.b.SourceAddressID)
        //                .Select(a => a.FromAddress)
        //                .FirstOrDefault() ?? "Address not available",

        //            ToLocation = _context.Address
        //                .Where(a => a.AddressID == joined.b.DestinationAddressID)
        //                .Select(a => a.ToAddress)
        //                .FirstOrDefault() ?? "Address not available",

        //            // Parse drop-down parameters dynamically
        //            TimeSlot = _context.TimeSlots
        //                .Where(ts => ts.TimeSlotID == joined.b.PickupTimeSlotID)
        //                .Select(ts => ts.TimeSlotName)
        //                .FirstOrDefault() ?? "Time Slot Pending",

        //            Type = joined.b.Isquotation == true ? "Quotation Check" : "Standard Packers & Movers"
        //        })
        //        .OrderByDescending(x => x.CreatedAt)
        //        .Cast<dynamic>()
        //        .ToListAsync();
        //}
    }

}
