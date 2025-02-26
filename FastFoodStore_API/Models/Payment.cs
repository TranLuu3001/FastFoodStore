using System.ComponentModel.DataAnnotations;

namespace FastFoodStore_API.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
    }
}
