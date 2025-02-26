using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_API.Models
{
    public class User: IdentityUser
    {
        //[Key]
        //public int UserId { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Column(TypeName = "nvarchar(100)")] 
        public string FullName { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Email không được để trống")]
        //[EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        //[StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        //[Column(TypeName = "nvarchar(100)")] 
        //public string Email { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Mật khẩu không được để trống")]
        //[StringLength(256, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        //[Column(TypeName = "nvarchar(256)")] 
        //public string PasswordHash { get; set; } = string.Empty;
        [NotMapped]
        //[Compare("PasswordHash", ErrorMessage ="Mật khẩu nhập lại sai")]
        public string Password { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Số điện thoại không được để trống")]
        //[Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        //[Column(TypeName = "nvarchar(15)")] 
        //public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự")]
        [Column(TypeName = "nvarchar(250)")] 
        public string Address { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự")]
        [Column(TypeName = "nvarchar(50)")]
        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Order>? Order { get; set; }

        public virtual ICollection<Cart>? Cart { get; set; }
        public virtual ICollection<Payment>? Payment { get; set; }
    }
}
