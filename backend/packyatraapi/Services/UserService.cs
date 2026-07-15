using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        // In UserService.cs
        public async Task<bool> UpdateUserAsync(Users user)
        {
            try
            {
                var existingUser = await _context.Users
             .FirstOrDefaultAsync(u => u.UserID == user.UserID);

                if (existingUser == null)
                    return false;

                // Update ONLY name and email
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;

                // Save changes
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error updating user: {ex.Message}");
                return false;
            }
        }
        public async Task<Users> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        /// ✅ **1. Add New Address**
        public async Task<Address> AddAddressAsync(Address address)
        {
            _context.Address.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        /// ✅ **2. Update Existing Address**
        public async Task<bool> UpdateAddressAsync(int id, Address updatedAddress)
        {
            var existingAddress = await _context.Address.FindAsync(id);
            if (existingAddress == null) return false;

            existingAddress.FromAddress = updatedAddress.FromAddress;
            existingAddress.ToAddress = updatedAddress.ToAddress;

            await _context.SaveChangesAsync();
            return true;
        }

        /// ✅ **3. Get Address by ID**
        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await _context.Address.FindAsync(id);
        }

        /// ✅ **4. Delete Address**
        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _context.Address .FindAsync(id);
            if (address == null) return false;

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
        // ✅ Step 1: Check if user exists by mobile number
        public async Task<Users> GetUserByMobileNumberAsync(string mobileNumber)
        {
          
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == mobileNumber);

           
        }

    }
}