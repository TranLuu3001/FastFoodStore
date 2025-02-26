using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodStore_API.Services
{
    public class ProductItemSevice : IProductItem
    {
        private readonly ApplicationDBContext _context;
        public ProductItemSevice(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryWithFoodItem>> GetCategoryWithFoodItemAsync()
        {
            return await _context.FoodItems.GroupBy(f => f.Category).Select(g => new CategoryWithFoodItem
            {
                Category = g.Key,
                FoodItems = g.ToList()
            }).ToListAsync();
        }
        public async Task<FoodItem> GetFoodItemByIdAsync(int id)
        {
            return await _context.FoodItems.FindAsync(id);
        }
    }
}
