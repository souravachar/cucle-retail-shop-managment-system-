using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Interfaces;
using CycleRetailShopAPI.Models;
using Microsoft.EntityFrameworkCore; // ✅ Required for ToListAsync

namespace CycleRetailShopAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetAdminEmails()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Admin)
                .Select(u => u.Email)
                .ToListAsync();
        }
    }
}
