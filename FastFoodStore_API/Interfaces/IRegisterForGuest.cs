

using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface IRegisterForGuest
    {
        Task<bool> RegisterForGuestAsync(User user);
        Task<bool> LoginAsync(string email, string password);
        void LogoutAsync();
    }
}
