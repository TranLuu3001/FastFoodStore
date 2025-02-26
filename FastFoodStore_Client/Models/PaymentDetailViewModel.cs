namespace FastFoodStore_Client.Models
{
    public class PaymentDetailViewModel
    {
        public string FullName { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
