using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using FastFoodStore_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodStore_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IProductItem _productItem;
        private readonly IGeneric<User> _generic;
        private readonly ICrudFoodItem _crudFoodItem;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly IPayment _payment;
        public HomeController(/*IRegisterForGuest registerForGuestService,*/ IProductItem productItem, IGeneric<User> generic, ICrudFoodItem crudFoodItem, UserManager<User> userManager, IEmailSender emailSender, SignInManager<User> signInManager, ApplicationDBContext context, IPayment payment)
        {
            //_registerForGuestService = registerForGuestService;
            _productItem = productItem;
            _generic = generic;
            _crudFoodItem = crudFoodItem;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _context = context;
            _payment = payment;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]User enduser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            // Kiểm tra tên người dùng đã tồn tại chưa
            if (!string.IsNullOrEmpty(enduser.FullName))
            {
                var existingUser = await _userManager.FindByNameAsync(enduser.FullName);
                if (existingUser != null)
                {
                    return BadRequest("Tên người dùng đã tồn tại");
                }
            }

            // Kiểm tra email đã tồn tại chưa
            if (!string.IsNullOrEmpty(enduser.Email))
            {
                var existingEmail = await _userManager.FindByEmailAsync(enduser.Email);
                if (existingEmail != null)
                {
                    return BadRequest("Email người dùng đã tồn tại");
                }
            }

            // Tạo đối tượng người dùng
            var user = new User
            {
                UserName = enduser.FullName,
                FullName = enduser.FullName!,
                Email = enduser.Email!,
                PhoneNumber = enduser.PhoneNumber,
                Role = "Customer",
                Address = enduser.Address,
                CreatedAt = enduser.CreatedAt,
            };

            // Tạo người dùng với mật khẩu
            var result = await _userManager.CreateAsync(user, enduser.Password);
            if (result.Succeeded)
            {
                // Thêm người dùng vào vai trò mặc định
                await _userManager.AddToRoleAsync(user, "Customer");

                // Tạo token xác nhận email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Home", new { userId = user.Id, token }, Request.Scheme);

                // Nội dung email
                var subject = "Xác nhận tài khoản của bạn";
                var message = $@"
                    <div style='font-family:Arial, sans-serif; color:#333; line-height:1.6;'>
                        <h2 style='color:#0056b3;'>Chào {user.UserName},</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản trên hệ thống của chúng tôi!</p>
                        <p>Vui lòng nhấp vào liên kết bên dưới để xác nhận tài khoản của bạn:</p>
                        <a href='{confirmationLink}' 
                           style='display:inline-block; background-color:#0056b3; color:#fff; padding:10px 20px; text-decoration:none; border-radius:5px;'>Xác nhận tài khoản</a>
                        <br><br>
                        <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                        <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>
                    </div>";

                // Gửi email xác nhận
                await _emailSender.SendEmailAsync(user.Email!, subject, message);

                return Ok("Tài khoản của bạn đã được tạo. Vui lòng kiểm tra email để xác nhận tài khoản.");
            }

            // Xử lý lỗi từ Identity
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tìm người dùng dựa trên Email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại.");
                return BadRequest("Email không tồn tại");
            }

            // Kiểm tra nếu tài khoản bị khóa
            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau.");
                return BadRequest("Tài khoản của bạn bị khoá");
            }

            // Kiểm tra nếu Email chưa được xác nhận
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Tài khoản chưa được xác nhận. Vui lòng kiểm tra email để xác nhận tài khoản và đăng nhập lại.");

                // Tạo mã xác nhận email
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Tạo liên kết xác nhận email
                var confirmationLink = Url.Action(
                    nameof(ConfirmEmail),
                    "Home",
                    new { userId = user.Id, token = confirmationToken },
                    Request.Scheme);

                // Nội dung email
                var subject = "Xác nhận tài khoản của bạn";
                var message = $@"
                    <div style='font-family:Arial, sans-serif; color:#333; line-height:1.6;'>
                        <h2 style='color:#0056b3;'>Chào {user.UserName},</h2>
                        <p>Vui lòng nhấp vào liên kết bên dưới để xác nhận tài khoản của bạn:</p>
                        <a href='{confirmationLink}' 
                           style='display:inline-block; background-color:#0056b3; color:#fff; padding:10px 20px; text-decoration:none; border-radius:5px;'>Xác nhận tài khoản</a>
                        <br><br>
                        <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                        <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>
                    </div>";

                try
                {
                    if (user.Email != null)
                    {
                        // Gửi email xác nhận
                        await _emailSender.SendEmailAsync(user.Email, subject, message);
                        Console.WriteLine("Email xác nhận tài khoản đã được gửi thành công.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                    // Ghi log lỗi hoặc xử lý bổ sung nếu cần
                }

                return NoContent();
            }
            try
            {
                // Xác thực tài khoản và mật khẩu
                var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    return Ok(new { UserId = user.Id, Name = user.FullName, Role = role });
                }
                else if (result.IsLockedOut)
                {
                    var subject = "Thông báo khóa tài khoản";
                    var message = $@"
                    <p>Chào {user.UserName},</p>
                    <p>Tài khoản của bạn đã bị khóa do nhập sai mật khẩu quá nhiều lần.</p>
                    <p>Bạn có thể thử lại sau 5 phút.</p>
                    <br>
                    <p>Trân trọng,</p>
                    <p>Đội ngũ hỗ trợ</p>";

                    if (user.Email != null)
                    {
                        await _emailSender.SendEmailAsync(email, subject, message);
                    }

                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng kiểm tra email để biết thêm thông tin.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Bạn không được phép đăng nhập vào lúc này.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin.");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng nhập: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.");
            }
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Thông tin xác nhận không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.Message = "Xác nhận email thành công. Bạn có thể đăng nhập ngay bây giờ.";
                return Ok("Xác nhận email thành công"); // Hiển thị trang xác nhận thành công
            }
            else
            {
                ViewBag.Message = "Xác nhận email không thành công. Liên kết có thể đã hết hạn hoặc không hợp lệ.";
                return Ok("Liên kết có thể hết hạn hoặc không hợp lệ"); // Hiển thị trang xác nhận thất bại
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var fooditem = await _productItem.GetFoodItemByIdAsync(id);
            if (fooditem == null) { NotFound(); }
            return Ok(fooditem);
        }
        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            var categories = await _productItem.GetCategoryWithFoodItemAsync();
            return Ok(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Cart(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Người dùng chưa đăng nhập");
            }
            var cart = await _crudFoodItem.GetCartItemAsync(userId);
            return Ok(new { cart });
        }
        //[HttpPost]
        //public async Task<IActionResult> AddPayment([FromBody] PaymentDetailViewModel newPayment)
        //{
        //    if (newPayment == null)
        //    {
        //        return BadRequest("Invalid payment information.");
        //    }

        //    var addedPayment = await _payment.AddNewPaymentAsync(newPayment);

        //    if (addedPayment == null)
        //    {
        //        return StatusCode(500, "Failed to create new payment.");
        //    }

        //    return Ok(addedPayment);
        //}

        [HttpPost]
        public async Task<IActionResult> AddCart(int FoodId, int quantity, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Người dùng chưa đăng nhập");
            }
            var result = await _crudFoodItem.AddToCartAsync(userId!, FoodId, quantity);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Thấy bại");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Đăng xuất thành công");
        }
        [HttpGet]
        public async Task<IActionResult> DetailUser(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Không tìm thấy người dùng");
            }

            var u = await _context.Users.FindAsync(id);

            if (u == null)
            {
                return NotFound();
            }

            return Ok(u);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.FindByIdAsync(user.Id);
                if(result == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                else
                {
                    result.FullName = user.FullName?? result.FullName;
                    result.Email = user.Email ?? result.Email;
                    result.Address = user.Address ?? result.Address;
                    result.CreatedAt = DateTime.Now;
                    result.Role = "Customer";   
                    result.NormalizedEmail = user.UserName !=null ? user.UserName.ToUpper() : result.NormalizedEmail;
                    result.NormalizedUserName = user.UserName != null ? user.UserName.ToUpper() : result.NormalizedUserName;
                    result.PasswordHash = user.Password ?? result.PasswordHash;
                    var update = await _userManager.UpdateAsync(result);
                    if(update.Succeeded)
                    {
                        return Ok("Cập nhật thông tin thành công");
                    }
                    else
                    {
                        return BadRequest("Cập nhật thông tin thất bại");
                    }
                }
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                TempData["ThongBao"] = $"Lỗi: {error.ErrorMessage}";
                return BadRequest("Lỗi: " + error.ErrorMessage);
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> PaymentIformation(string id)
        {
            var pmifo = await _payment.GetByIdAsync(id);  
            if (pmifo == null || !pmifo.Any())
            {
                return NotFound(); 
            }
            return Ok(pmifo); 
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFoodInCart(int foodItemId, int quantity, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Người dùng chưa đăng nhập");
            }
            var result = await _crudFoodItem.DeleteFoodInCart(userId, foodItemId, quantity);
            if (result)
            {
                return Ok("xoá sp thành công");
            }
            return NotFound();
        }
    }
}
