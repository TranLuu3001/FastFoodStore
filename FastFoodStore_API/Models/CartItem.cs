namespace FastFoodStore_API.Models
{
    public class CartItem
    {
        public int CartId { get; set; }
        public string FoodName { get; set; }
        public int FoodItemId { get; set; }
        public string UserId { get; set; }
        public int Quatity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    
}
