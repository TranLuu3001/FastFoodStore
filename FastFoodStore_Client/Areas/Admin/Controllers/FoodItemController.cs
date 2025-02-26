using FastFoodStore_Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using FastFoodStore_Client.Attribute;


namespace FastFoodStore_Client.Controllers
{
    [Area("Admin")]
    [CheckUserLogin]
    public class FoodItemController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;

        public FoodItemController(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodStore_API");
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("Admin/FoodItem/Index");
                var food = JsonConvert.DeserializeObject<List<FoodItem>>(response);
                return View(food);
            }
            catch (Exception)
            {
                return View(new List<FoodItem>());
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/FoodItem/Details/{id}");
            var food = JsonConvert.DeserializeObject<FoodItem>(response);
            return View(food);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FoodItem foodItem, IFormFile? imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return View(foodItem);
                }
                // Tạo MultipartFormDataContent để chứa dữ liệu
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các trường của FoodItem vào form
                    content.Add(new StringContent(foodItem.FoodName ?? string.Empty), "FoodName");
                    content.Add(new StringContent(foodItem.Category ?? string.Empty), "Category");
                    content.Add(new StringContent(foodItem.Price.ToString()), "Price");
                    content.Add(new StringContent(foodItem.Description ?? string.Empty), "Description");
                    var imageFileName = Path.GetFileName(imageFile!.FileName); // Lấy tên file
                    var response = await _httpClient.PostAsync($"Admin/FoodItem/Create?imageFileName={imageFileName}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await SaveImageAsync(imageFile); // Lưu ảnh vào thư mục
                        return RedirectToAction(nameof(Index)); // Thành công, điều hướng về Index
                    }
                    else
                    {
                        // Đọc nội dung lỗi nếu có
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Lỗi từ API: " + errorContent);
                    }
                }

                return View(foodItem);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối API: " + ex.Message;
            }
            catch (JsonException)
            {
                ViewBag.ErrorMessage = "Lỗi xử lý dữ liệu từ API.";
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tạo đơn hàng.", error = ex.Message });
            }
            return View(foodItem);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            // Đường dẫn đến thư mục food_menu
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/food_menu");

            // Kiểm tra nếu thư mục chưa tồn tại thì tạo mới
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string filePath = Path.Combine(uploadsFolder, imageFile.FileName);

            // Lưu ảnh vào thư mục
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            // Trả về đường dẫn ảnh (relatively to the web root)
            return imageFile.FileName;
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/FoodItem/Details/{id}");
            var food = JsonConvert.DeserializeObject<FoodItem>(response);
            return View(food);
        }

        public async Task<IActionResult> Edit(int id, FoodItem foodItem, IFormFile? imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return View(foodItem);
                }

                // Lấy tên file hình ảnh (nếu có)
                string imageFileName = imageFile != null ? imageFile.FileName : string.Empty;

                if (string.IsNullOrEmpty(imageFileName))
                {
                    ModelState.AddModelError("ImageFile", "Hình ảnh không được để trống.");
                    return View(foodItem); 
                }

                // Tạo MultipartFormDataContent để chứa dữ liệu
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các trường của FoodItem vào form
                    content.Add(new StringContent(foodItem.FoodName), "FoodName");
                    content.Add(new StringContent(foodItem.Category), "Category");
                    content.Add(new StringContent(foodItem.Price.ToString()), "Price");
                    content.Add(new StringContent(foodItem.Description ?? string.Empty), "Description");
                    content.Add(new StringContent(imageFileName), "imageFileName"); 
                    var response = await _httpClient.PutAsync($"Admin/FoodItem/Edit/{id}?imageFileName={imageFileName}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Đọc nội dung lỗi nếu có
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorContent);
                    }
                }

                return View(foodItem);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối API: " + ex.Message;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi chỉnh sửa sản phẩm.", error = ex.Message });
            }

            return View(foodItem);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/FoodItem/Details/{id}");
            var food = JsonConvert.DeserializeObject<FoodItem>(response);
            return View(food);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Admin/FoodItem/DeleteConfirmed/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorContent);
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối API: " + ex.Message;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm.", error = ex.Message });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
