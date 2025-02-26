using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodStore_API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILoginForAdmin _loginForAdmin;

        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;
        private readonly ICrudUser _crudUser;
        public HomeController(ILoginForAdmin loginForAdmin, UserManager<User> userManager, SignInManager<User> signInManager, ICrudUser crudUser)
        {
            _loginForAdmin = loginForAdmin;
            _userManager = userManager;
            _signInManager = signInManager;
            _crudUser = crudUser;
        }
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _crudUser.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Đăng xuất thành công");
        }
        //[Route("/Admin")]
        //public IActionResult LoginRedirect()
        //{
        //    return RedirectToAction("Login");
        //}
    }
}
