using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_API.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodId { get; set; }

        [Required(ErrorMessage = "Tên món ăn không được để trống")]
        [StringLength(100, ErrorMessage = "Tên món ăn không được vượt quá 100 ký tự")]
        public string FoodName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá món ăn không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục món ăn không được để trống")]
        [StringLength(50, ErrorMessage = "Danh mục không được vượt quá 50 ký tự")]
        public string Category { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày tạo không được để trống")]
        public DateTime CreatedAt { get; set; }

        // Quan hệ một-nhiều: một món ăn có thể có nhiều ComboItem
        public virtual ICollection<ComboItem>? ComboItem { get; set; }

        // Quan hệ một-nhiều: một món ăn có thể có nhiều OrderDetails
        public virtual ICollection<OrderDetail>? OrderDetail { get; set; }
        public virtual ICollection<Cart>? Cart { get; set; }
    }
    public class CategoryWithFoodItem
    {
        public string Category { get; set; }
        public List<FoodItem> FoodItems { get; set; }
    }
}
