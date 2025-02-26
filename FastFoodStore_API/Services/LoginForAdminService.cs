using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodStore_API.Services
{
    public class LoginForAdminService:ILoginForAdmin
    {
        private readonly ApplicationDBContext _context;
        public LoginForAdminService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<User?> LoginAd(string email, string password)
        {
            var user = await _context.Users
           .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            return user?.Role == "Admin" ? user : null;
        }
    }
}
