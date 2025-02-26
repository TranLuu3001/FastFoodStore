

using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface ICrudOrder
    {
        Task<IEnumerable<Order>> GetAllAsync();
    }
}
