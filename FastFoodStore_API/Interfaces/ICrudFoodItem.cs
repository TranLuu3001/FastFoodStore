
using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface ICrudFoodItem
    {
        Task<IEnumerable<FoodItem>> GetAllAsync();
        Task<FoodItem?> GetByIdAsync(int id);
        Task CreateAsync(FoodItem foodItem);
        Task UpdateAsync(int id, FoodItem foodItem, string image);
        Task DeleteAsync(int id);
        Task<List<CartItem>> GetCartItemAsync(string userId);
        Task<bool> AddToCartAsync(string userId, int foodItemId, int quantity);
        Task<bool> DeleteFoodInCart(string userId, int foodItemId, int quantity);
    }
}
