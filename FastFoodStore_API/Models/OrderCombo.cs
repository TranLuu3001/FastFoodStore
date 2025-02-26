using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_API.Models
{
    public class OrderCombo
    {
        [Key]
        public int OrderComboId { get; set; } // Khóa chính

        [Required(ErrorMessage = "OrderId không được để trống")]
        [ForeignKey("Orders")]  // Chỉ định quan hệ khóa ngoại với bảng Order
        public int OrderId { get; set; } // Khóa ngoại từ Order
      
        public virtual Order? Order { get; set; }

        [Required(ErrorMessage = "ComboId không được để trống")]
        [ForeignKey("Combos")]  // Chỉ định quan hệ khóa ngoại với bảng Combo
        public int ComboId { get; set; } // Khóa ngoại từ Combo

        public virtual Combo? Combo { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }
    }
}
