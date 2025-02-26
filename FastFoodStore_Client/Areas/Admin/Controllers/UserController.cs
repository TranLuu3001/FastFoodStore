using FastFoodStore_Client.Attribute;
using FastFoodStore_Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FastFoodStore_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckUserLogin]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodStore_API");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("Admin/User/Index");
                var listuser = JsonConvert.DeserializeObject<List<User>>(response);
                return View(listuser);
            }
            catch (Exception)
            {
                return View(new List<User>());
            }
        }

        public async Task<IActionResult> Detail(string id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/User/Detail/{id}");
            var detailuser = JsonConvert.DeserializeObject<User>(response);
            return View(detailuser);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User enduser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", enduser);
                }
                if (enduser == null)
                {
                    return BadRequest();
                }
                //var content = new StringContent(JsonConvert.SerializeObject(enduser), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsJsonAsync($"Admin/User/Create", enduser);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View("Create", enduser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> Update(string id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/User/Detail/{id}");
            var user = JsonConvert.DeserializeObject<User>(response);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Update(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", user);
                }
                if (user == null)
                {
                    return BadRequest();
                }
                var response = await _httpClient.PutAsJsonAsync($"Admin/User/Update?id={user.Id}", user);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View("Update", user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.GetStringAsync($"Admin/User/Detail/{id}");
            var user = JsonConvert.DeserializeObject<User>(response);
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Admin/User/DeleteConfirmed/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View("Delete", id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //public async Task<IActionResult> Delete(string id)
        //{
        //    var user = await _crudUser.GetUserByIdAsync(id);
        //    if (user == null) return NotFound();
        //    return View(user);
        //}

        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    await _crudUser.DeleteUserAsync(id);
        //    return RedirectToAction("Index");
        //}
    }
}
