using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_API.Models
{
    public class Combo
    {
        [Key]
        public int ComboId { get; set; }

        [Required(ErrorMessage = "Tên Combo không được để trống")]
        [StringLength(100, ErrorMessage = "Tên Combo không được vượt quá 100 ký tự")]
        public string ComboName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá của Combo không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "money")]  
        public decimal Price { get; set; }
        public string ImageURL { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày tạo không được để trống")]
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<ComboItem>? ComboItem { get; set; }

        // Quan hệ một-nhiều: một Combo có thể có nhiều OrderCombos
        public virtual ICollection<OrderCombo>? OrderCombo { get; set; }
    }
}
