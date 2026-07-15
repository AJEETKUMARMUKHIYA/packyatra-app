using MoversAndPackerApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoversAndPackerApi.Data;

namespace MoversAndPackerApi.Services
{
    public class AdminUserService
    {
        private readonly ApplicationDbContext _context;

        public AdminUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserAdmin>> GetUsersAsync()
        {
            return await _context.UserAdmin
                
                .Select(u => new UserAdmin
                {
                    UserId = u.UserId,
                 
                    RoleId = u.RoleId,
                    Password = u.Password,
                    Email = u.Email,
                    Mobile = u.Mobile,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    //UpdatedBy = u.UpdatedBy,
                    UpdatedDate = u.UpdatedDate,
                    Active = u.Active,
                    LastActivityDate = u.LastActivityDate,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                 
                    DefaultAccountId = u.DefaultAccountId
                })
                .ToListAsync();
        }

        public async Task<UserAdmin> CreateUserAsync(UserAdmin user)
        {
            try
            {
                if (await _context.UserAdmin.AnyAsync(u => u.Email == user.Email))
                    throw new Exception("Email already exists");

                user.Email = user.Email;
                user.RoleId = user.RoleId;
                user.CreatedBy = user.CreatedBy;
                user.UpdatedBy = user.UpdatedBy;
                user.CreatedDate = DateTime.UtcNow;
                user.UpdatedDate = DateTime.UtcNow;
                user.Active = true;
                user.LastActivityDate = DateTime.UtcNow;
                //user.FullName = $"{user.FirstName} {user.LastName}";

                _context.UserAdmin.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(UserAdmin user)
        {
            try
            {
                var existingUser = await _context.UserAdmin
                    .FirstOrDefaultAsync(u => u.UserId == user.UserId && (u.RoleId == 1 || u.RoleId == 2));
                if (existingUser == null)
                    return false;

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
               
                existingUser.Email = user.Email;
                existingUser.Email = user.Email;
                existingUser.Mobile = user.Mobile;
                existingUser.Password = user.Password; // Consider hashing
                //existingUser.DefaultAccountId = user.DefaultAccountId;
                //existingUser.RoleId = user.RoleId;
                //existingUser.UpdatedBy = user.UpdatedBy;
                existingUser.UpdatedDate = DateTime.UtcNow;
                existingUser.Active = user.Active;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        //public async Task<bool> DeleteUserAsync(int userId)
        //{
        //    try
        //    {
        //        var user = await _context.UserAdmin
        //            .FirstOrDefaultAsync(u => u.UserId == userId && (u.RoleId == 1 || u.RoleId == 2));
        //        if (user == null)
        //            return false;

        //        _context.UserAdmin.Remove(user);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error deleting user: {ex.Message}");
        //        return false;
        //    }
        //}
        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.UserAdmin
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null || !user.Active)   // already inactive?
                    return false;

                user.Active = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating user: {ex.Message}");
                return false;
            }
        }
        public async Task<UserAdmin> ValidateUserAsync(string firstName, string password)
        {
            try
            {
                var user = await _context.UserAdmin
                    .FirstOrDefaultAsync(u =>
                        u.FirstName == firstName &&
                        u.Password == password &&
                    
                        u.Active == true);
                return user; // Returns null if credentials are invalid or user is not active
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating user: {ex.Message}");
                throw;
            }
        }
    }
}