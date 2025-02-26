using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface ICrudCombo
    {
        Task<IEnumerable<Combo>> GetAllAsync();
        Task<Combo?> GetByIdAsync(int id);
        Task CreateAsync(Combo combo);
        Task UpdateAsync(Combo combo, string imageFile, int id);
        Task DeleteAsync(int id);
    }
}
