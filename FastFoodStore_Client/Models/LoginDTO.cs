namespace FastFoodStore_Client.Models
{
    public class LoginDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool? RememberMe { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string?  Role { get; set; }
    }
}
