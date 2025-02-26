using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodStore_API.Services
{
    public class PaymentService:IPayment
    {
        private readonly ApplicationDBContext _context;
        public PaymentService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<PaymentDetailViewModel>> GetByIdAsync(string id)
        {
            var payment = await _context.Payment
                .Include(p => p.User)  
                .Include(p => p.Order) 
                .Where(p => p.UserId == id)
                .Select(p => new PaymentDetailViewModel
                {
                    FullName = p.User.FullName, // Lấy thông tin họ tên từ User
                    PaymentDate = p.PaymentDate,
                    PaymentStatus = p.PaymentStatus,
                    PaymentMethod = p.PaymentMethod,
                    OrderId = p.OrderId,
                    UserId = p.UserId,
                    PaymentId = p.PaymentId,
                    DeliveryAddress = p.DeliveryAddress
                })
                .ToListAsync();

            return payment;
        }
        public async Task<PaymentDetailViewModel> AddNewPaymentAsync(PaymentDetailViewModel newPayment)
        {
            var payment = new Payment//tạo mới payment để thêm vào db
            {
                PaymentDate = newPayment.PaymentDate,
                PaymentStatus = newPayment.PaymentStatus,
                PaymentMethod = newPayment.PaymentMethod,
                UserId = newPayment.UserId,
                DeliveryAddress = newPayment.DeliveryAddress
            };
            _context.Payment.Add(payment);
            await _context.SaveChangesAsync(); 

            return newPayment; //vì bảng trong db là payment nên trả về payment, còn mình sử dụng PaymentDetailViewModel để truyền dữ liệu nên phải tạo payment mới để truyền dữ liệu
            /*Nên k làm được kiểu giống như vậy
             *  public async Task CreateAsync(PaymentDetailViewModel newPayment)
                {
                    newPayment.PaymentDate = DateTime.Now;
                    _context.Combos.Add(newPayment);
                    await _context.SaveChangesAsync();
                }*/
        }
    }
}
