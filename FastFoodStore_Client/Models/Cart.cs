using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_Client.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public virtual FoodItem? FoodItem { get; set; }
        public int FoodItemId { get; set; }
        public virtual User? User { get; set; }
        public string UserID { get; set; }
        public int Quatity { get; set; }

        [Column(TypeName = "money")]
        public decimal Total { get; set; }
    }
}
