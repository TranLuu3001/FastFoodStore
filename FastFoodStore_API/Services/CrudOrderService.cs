using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodStore_API.Services
{
    public class CrudOrderService : ICrudOrder
    {
        private readonly ApplicationDBContext _context;
        public CrudOrderService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }
    }
}
