using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodStore_API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ICrudUser _crudUser;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public UserController(ICrudUser crudUser, UserManager<User> userManager, IEmailSender emailSender)
        {
            _crudUser = crudUser;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _crudUser.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _crudUser.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User enduser)
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] User user, string id)
        {
            
            if (ModelState.IsValid)
            {
                var result = await _userManager.FindByIdAsync(id);
                if (result == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                else
                {
                    result.FullName = user.FullName ?? result.FullName;
                    result.Email = user.Email ?? result.Email;
                    result.Address = user.Address ?? result.Address;
                    result.PhoneNumber = user.PhoneNumber ?? result.PhoneNumber;
                    result.CreatedAt = DateTime.Now;
                    result.Role = "Customer";
                    result.NormalizedEmail = user.UserName != null ? user.UserName.ToUpper() : result.NormalizedEmail;
                    result.NormalizedUserName = user.UserName != null ? user.UserName.ToUpper() : result.NormalizedUserName;
                    result.PasswordHash = user.Password ?? result.PasswordHash;
                    var update = await _userManager.UpdateAsync(result);
                    if (update.Succeeded)
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
                return BadRequest("Lỗi: " + error.ErrorMessage);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var exit = await _crudUser.GetUserByIdAsync(id);
            if (exit == null)
            {
                return NotFound();
            }
            await _crudUser.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
