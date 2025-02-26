﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFoodStore_Client.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; } // Khóa chính

        [Required(ErrorMessage = "OrderId không được để trống")]
        [ForeignKey("Orders")]  // Chỉ định quan hệ khóa ngoại với bảng Order
        public int OrderId { get; set; } // Khóa ngoại từ Order
        public virtual Order? Order { get; set; }

        [Required(ErrorMessage = "FoodId không được để trống")]
        [ForeignKey("FoodItems")]  // Chỉ định quan hệ khóa ngoại với bảng FoodItem
        public int FoodId { get; set; } // Khóa ngoại từ FoodItem
         public virtual FoodItem? FoodItem { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
    }
}
