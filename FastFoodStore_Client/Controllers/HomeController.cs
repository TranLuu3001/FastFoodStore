using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FastFoodStore_Client.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace FastFoodStore_Client.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("FastFoodStore_API");

    }

    public IActionResult Index()
    {
        return View();
    }
    public  IActionResult About()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        var userId = HttpContext.Session.GetString("EndUserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login");
        }

        var response = await _httpClient.GetStringAsync($"home/Cart?userId={userId}");
        var hehehehehehehhehehehe= JsonConvert.DeserializeObject<CartItemDto>(response);

        if (hehehehehehehhehehehe!= null)
        {
            return View(hehehehehehehhehehehe.Cart);
        }

        ModelState.AddModelError(string.Empty, "Không thể tải giỏ hàng. Vui lòng thử lại.");
        return View(); 
    }
    //public async Task<IActionResult> CreateNewPayment(PaymentDetailViewModel newPayment)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(newPayment); 
    //    }

    //    var response = await _httpClient.PostAsJsonAsync("home/AddPayment", newPayment);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var jsonData = await response.Content.ReadAsStringAsync();
    //        var addedPayment = JsonConvert.DeserializeObject<PaymentDetailViewModel>(jsonData);

    //        return RedirectToAction("PaymentSuccess", new { message = "Payment created successfully!" });
    //    }

    //    return View(newPayment); 
    //}

    [HttpPost]
    public async Task<IActionResult> AddCart(int FoodId, int quantity)
    {
        var userId = HttpContext.Session.GetString("EndUserId");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Người dùng chưa đăng nhập.");
        }

        var cartData = new { UserId = userId, FoodItem = FoodId, Quantity = quantity };

        var response = await _httpClient.PostAsJsonAsync($"Home/AddCart?FoodId={FoodId}&quantity={quantity}&userId={userId}",cartData);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Thêm vào giỏ hàng thành công!";
            return RedirectToAction("Cart");
        }
        else
        {
            TempData["ErrorMessage"] = "Không thể thêm vào giỏ hàng. Vui lòng thử lại.";
            return RedirectToAction("Cart");
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteFoodInCart(int foodItemId, int quantity)
    {
        var userId = HttpContext.Session.GetString("EndUserId");

        if (string.IsNullOrEmpty(userId))
        {
            TempData["ErrorMessage"] = "Người dùng chưa đăng nhập.";
            return RedirectToAction("Cart");
        }

        var cartData = new { UserId = userId, FoodItem = foodItemId, Quantity = quantity };

        var response = await _httpClient.DeleteAsync($"Home/DeleteFoodInCart?foodItemId={foodItemId}&quantity={quantity}&userId={userId}");

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Xoá sản phẩm thành công!";
            return RedirectToAction("Cart");
        }
        else
        {
            TempData["ErrorMessage"] = "Không thể xoá sản phẩm. Vui lòng thử lại.";
            return RedirectToAction("Cart");
        }
    }
    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var response = await _httpClient.GetStringAsync($"Home/Detail?id={id}");
        var fooditem = JsonConvert.DeserializeObject<FoodItem>(response); 

            if (fooditem == null)
            {
                return NotFound();
            }
            return View(fooditem);
    }
    [HttpGet]
    public async Task<IActionResult> DetailUser()
    {
        try
        {
            var enduserId = HttpContext.Session.GetString("EndUserId");
            if (enduserId == null)
            {
                return RedirectToAction("Login");
            }
            var response = await _httpClient.GetStringAsync($"Home/DetailUser?id={enduserId}");
            var user = JsonConvert.DeserializeObject<User>(response);
            if (user == null)
            {
                return View(new User());
            }
            return View(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> PaymentInformation()
    {
        var id = HttpContext.Session.GetString("EndUserId");

        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Invalid user ID in session.");
        }

        var response = await _httpClient.GetAsync($"Home/PaymentIformation?id={id}");

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();

            var paymentInfoList = JsonConvert.DeserializeObject<List<PaymentDetailViewModel>>(jsonData);

            return View(paymentInfoList);
        }

        return NotFound();
    }


    [HttpGet]
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
            var response = await _httpClient.PostAsync($"Home/Login?email={email}&password={password}&rememberMe={rememberMe}",null);
            
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginDTO>();
                if (result != null)
                {
                    HttpContext.Session.SetString("EndUser", result.Name);
                    HttpContext.Session.SetString("EndUserId", result.UserId);
                    HttpContext.Session.SetString("EndUserRole", result.Role);
                    return RedirectToAction("Index", "Home");
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
    public async Task<IActionResult> Register(User enduser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View("Login", enduser);
            }
            if (enduser == null)
            {
                return BadRequest();
            }
            //var content = new StringContent(JsonConvert.SerializeObject(enduser), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsJsonAsync($"Home/Register",enduser);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View("Login", enduser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
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

    [HttpGet]
    public async Task<IActionResult> Menu()
    {
        try
        {
            var response = await _httpClient.GetAsync("Home/Menu");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var menu = JsonConvert.DeserializeObject<List<CategoryWithFoodItem>>(content);
                return View(menu);
            }
            else
            {
                return BadRequest("Không thể lấy danh sách Combo từ API.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Có lỗi xảy ra: {ex.Message}");

        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateCustomer(User enduser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View("DetailUser", enduser);
            }
            if (enduser == null)
            {
                return BadRequest();
            }
            var response = await _httpClient.PutAsJsonAsync($"Home/UpdateCustomer", enduser);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(DetailUser));
            }
            return View("DetailUser", enduser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
