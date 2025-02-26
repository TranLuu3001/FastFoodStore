

using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface IProductItem
    {
        Task<List<CategoryWithFoodItem>> GetCategoryWithFoodItemAsync();
        Task<FoodItem> GetFoodItemByIdAsync(int id);
    }
}
