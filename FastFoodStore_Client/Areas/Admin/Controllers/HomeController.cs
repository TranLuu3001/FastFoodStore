using FastFoodStore_Client.Attribute;
using FastFoodStore_Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodStore_API");

        }
        [CheckUserLogin]
        public IActionResult Index()
        {
            var rolead = HttpContext.Session.GetString("AdminRole");
            if (rolead != "Admin")
            {
                RedirectToAction("Login");
            }
            return View();
                //var role = HttpContext.Session.GetString("AdminRole");
                //if (string.IsNullOrEmpty(role))
                //{
                //    return RedirectToAction("Login", "Home", new { area = "Admin" });
                //}
        }
        public async Task<IActionResult> Detail(string id)
        {
            id = HttpContext.Session.GetString("AdminId");
            var response = await _httpClient.GetStringAsync($"Admin/Home/Detail?id={id}");
            var detailuser = JsonConvert.DeserializeObject<User>(response);
            return View(detailuser);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                    return View("Login");
                }

                // Gửi yêu cầu POST tới API để xử lý đăng nhập
                var response = await _httpClient.PostAsync($"Home/Login?email={email}&password={password}&rememberMe={rememberMe}", null);


                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginDTO>();
                    if (result != null)
                    {
                        HttpContext.Session.SetString("AdminName", result.Name);
                        HttpContext.Session.SetString("AdminId", result.UserId);
                        HttpContext.Session.SetString("AdminRole", result.Role);
                        var find = HttpContext.Session.GetString("AdminRole");
                        if (find != "Admin")
                        {
                            HttpContext.Session.Clear();
                            return View("Login");
                        }
                        
                        return View("Index", "Home");
                    }
                }
                else
                {
                    // Lấy nội dung lỗi từ phản hồi API
                    var errorContent = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        // Nếu API trả về lỗi đăng nhập, thêm thông báo lỗi và nội dung chi tiết vào ModelState
                        ModelState.AddModelError(string.Empty, $"Đăng nhập không thành công. Chi tiết: {errorContent}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Locked)
                    {
                        // Nếu tài khoản bị khóa, hiển thị thông báo tương ứng
                        ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa.");
                    }
                    else
                    {
                        // Nếu có mã lỗi khác, hiển thị mã lỗi và nội dung chi tiết
                        ModelState.AddModelError(string.Empty, $"Lỗi: {response.StatusCode}. Chi tiết: {errorContent}");
                    }
                }

                return View("Login");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
                return View("Login");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _httpClient.PostAsync("home/logout", null);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Clear();
                    return View("Login");
                }
                else
                {
                    return BadRequest("Đăng xuất không thành công.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }
        [Route("/Admin")]
        public IActionResult LoginRedirect()
        {
            return RedirectToAction("Login");
        }
    }
}
