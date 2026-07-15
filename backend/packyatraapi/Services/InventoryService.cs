using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Services
{
    public class InventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryItem>> GetAllInventoryItemsAsync()
        {
            return await _context.MasterInventory.ToListAsync();
        }

        public async Task<InventoryItem> GetInventoryItemByIdAsync(int id)
        {
            return await _context.MasterInventory.FindAsync(id);
        }

        public async Task<List<InventoryItem>> GetInventoryItemsByCategoryAsync(string category)
        {
            return await _context.MasterInventory.Where(i => i.Category == category).ToListAsync();
        }
    }
}