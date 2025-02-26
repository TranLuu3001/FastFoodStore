

using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface ICrudUser
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
    }
}
