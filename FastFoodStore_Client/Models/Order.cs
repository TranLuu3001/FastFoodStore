using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_Client.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Khóa chính


        [Required(ErrorMessage = "UserId không được để trống")]

        [ForeignKey("Users")]
        public string Users { get; set; } // Khóa ngoại từ User

        public virtual User? User { get; set; }

        [Required(ErrorMessage = "Trạng thái đơn hàng không được để trống")]
        [StringLength(50, ErrorMessage = "Trạng thái đơn hàng không được vượt quá 50 ký tự")]
        public string OrderStatus { get; set; } = string.Empty; // Trạng thái đơn hàng: Pending, Shipped, Delivered, Cancelled

        [Required(ErrorMessage = "Ngày đặt đơn không được để trống")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Tổng số tiền không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tổng số tiền phải lớn hơn 0")]
        [Column(TypeName = "money")]
        public decimal TotalAmount { get; set; }

        // Quan hệ một-nhiều: một Order có nhiều OrderDetails
        public virtual ICollection<OrderDetail>? OrderDetail { get; set; }

        // Quan hệ một-nhiều: một Order có nhiều OrderCombos
        public virtual ICollection<OrderCombo>? OrderCombo { get; set; }
        public virtual ICollection<Payment>? Payment { get; set; }
    }
}
