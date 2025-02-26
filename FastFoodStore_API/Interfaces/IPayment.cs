using FastFoodStore_API.Models;

namespace FastFoodStore_API.Interfaces
{
    public interface IPayment
    {
        Task<List<PaymentDetailViewModel>> GetByIdAsync(string id);
        Task<PaymentDetailViewModel> AddNewPaymentAsync(PaymentDetailViewModel newPayment);
    }
}
