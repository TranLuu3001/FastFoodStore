using FastFoodStore_API.Models;

namespace FastFoodStore_API.Services
{
    public class UserService : GenericService<User>
    {
        public UserService(ApplicationDBContext context) : base(context)
        {
        }
    }
}
