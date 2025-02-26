using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_Client.Models
{
    public class ComboItem
    {
        [Key]
        public int CombomItemId { get; set; }

        [Required(ErrorMessage = "ComboId không được để trống")]
        [ForeignKey("Combos")]
        public int ComboId { get; set; } // Khóa ngoại từ Combo

        public virtual Combo? Combo { get; set; }

        [Required(ErrorMessage = "FoodId không được để trống")]
        [ForeignKey("FoodItems")]
        public int FoodId { get; set; } // Khóa ngoại từ FoodItem

        public virtual FoodItem? FoodItem { get; set; }
    }
}
