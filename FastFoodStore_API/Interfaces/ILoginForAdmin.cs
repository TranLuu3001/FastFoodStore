

using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface ILoginForAdmin
    {
        Task<User>? LoginAd(string email, string password);   
    }
}
