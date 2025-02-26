using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_Client.Models
{
    public class SearchHistory
    {
        [Key]
        public int SearchId { get; set; } // Khóa chính

        [Required(ErrorMessage = "UserId không được để trống")]
        [ForeignKey("Users")]  // Chỉ định quan hệ khóa ngoại với bảng User
        public string UserId { get; set; } // Khóa ngoại từ User
        public virtual User? User { get; set; }

        [Required(ErrorMessage = "Câu tìm kiếm không được để trống")]
        [StringLength(500, ErrorMessage = "Câu tìm kiếm không được vượt quá 500 ký tự")]
        public string SearchQuery { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày tìm kiếm không được để trống")]
        public DateTime SearchDate { get; set; } = DateTime.Now;
    }
}

