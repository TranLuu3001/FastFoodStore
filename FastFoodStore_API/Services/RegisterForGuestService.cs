using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace FastFoodStore_API.Services
{
    public class RegisterForGuestService : IRegisterForGuest
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public RegisterForGuestService(ApplicationDBContext context, IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _context = context;
        }
        public async Task<bool> RegisterForGuestAsync(User user)
        {
            if (await _context.Users.AnyAsync(guest => guest.Email == user.Email))
            {
                return false;
            }
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = "Customer";
                user.CreatedAt = DateTime.Now;
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.PasswordHash!=password)
            {
                return false;
            }
            _contextAccessor.HttpContext.Response.Cookies.Append("UserId",user.Id.ToString(), new CookieOptions
            {
                IsEssential= true,
            });
            _contextAccessor.HttpContext.Response.Cookies.Append("Role", user.Role, new CookieOptions
            {
                IsEssential = true,
            });
            return true;
        }
        public void LogoutAsync()
        {
            _contextAccessor.HttpContext.Response.Cookies.Delete("UserId");
            _contextAccessor.HttpContext.Response.Cookies.Delete("Role");
        }
    }
}
