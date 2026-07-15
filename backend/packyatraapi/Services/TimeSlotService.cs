using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoversAndPackerApi.Data;

namespace MoversAndPackerApi.Services
{
    public class TimeSlotService
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetAllTimeSlotsAsync()
        {
            return await _context.TimeSlots.ToListAsync();
        }

        public async Task<TimeSlot> GetTimeSlotByIdAsync(int id)
        {
            return await _context.TimeSlots
                .FirstOrDefaultAsync(t => t.TimeSlotID == id);
        }
    }
}
