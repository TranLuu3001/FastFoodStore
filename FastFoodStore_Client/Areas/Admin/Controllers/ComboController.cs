using FastFoodStore_Client.Models;
using FastFoodStore_Client.Attribute;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FastFoodStore_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckUserLogin]
    public class ComboController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;

        public ComboController(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodStore_API");
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("Admin/Combo/Index");
                var combo = JsonConvert.DeserializeObject<List<Combo>>(response);
                return View(combo);
            }
            catch (Exception)
            {
                return View(new List<Combo>());
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/Combo/Details/{id}");
            var combo = JsonConvert.DeserializeObject<Combo>(response);
            return View(combo);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Combo combo, IFormFile? imageFile)
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
                    return View(combo);
                }
                // Tạo MultipartFormDataContent để chứa dữ liệu
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các trường của FoodItem vào form
                    content.Add(new StringContent(combo.ComboName ?? string.Empty), "ComboName");
                    content.Add(new StringContent(combo.Description ?? string.Empty), "Description");
                    content.Add(new StringContent(combo.Price.ToString()), "Price");
                    var imageFileName = Path.GetFileName(imageFile!.FileName); 
                    var response = await _httpClient.PostAsync($"Admin/Combo/Create?imageFileName={imageFileName}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await SaveImageAsync(imageFile); 
                        return RedirectToAction(nameof(Index)); 
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Lỗi từ API: " + errorContent);
                    }
                }

                return View(combo);
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
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tạo combo.", error = ex.Message });
            }
            return View(combo);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            // Đường dẫn đến thư mục food_menu
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/food_item");

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
            var response = await _httpClient.GetStringAsync($"Admin/Combo/Details/{id}");
            var combo = JsonConvert.DeserializeObject<Combo>(response);
            return View(combo);
        }

        public async Task<IActionResult> Edit(int id, Combo combo, IFormFile? imageFile)
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
                    return View(combo);
                }

                // Lấy tên file hình ảnh (nếu có)
                string imageFileName = imageFile != null ? imageFile.FileName : string.Empty;

                if (string.IsNullOrEmpty(imageFileName))
                {
                    ModelState.AddModelError("ImageFile", "Hình ảnh không được để trống.");
                    return View(combo);
                }

                // Tạo MultipartFormDataContent để chứa dữ liệu
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các trường của FoodItem vào form
                    content.Add(new StringContent(combo.ComboName ?? string.Empty), "ComboName");
                    content.Add(new StringContent(combo.Description ?? string.Empty), "Description");
                    content.Add(new StringContent(combo.Price.ToString()), "Price");
                    content.Add(new StringContent(imageFileName), "imageFileName");
                    var response = await _httpClient.PutAsync($"Admin/Combo/Edit/{id}?imageFileName={imageFileName}", content);

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

                return View(combo);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối API: " + ex.Message;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi chỉnh sửa sản phẩm.", error = ex.Message });
            }

            return View(combo);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/Combo/Details/{id}");
            var combo = JsonConvert.DeserializeObject<Combo>(response);
            return View(combo);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Admin/Combo/DeleteConfirmed/{id}");

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
