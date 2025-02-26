using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FastFoodStore_API.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    public class FoodItemController : Controller
    {
        private readonly ICrudFoodItem _crudFoodItem;

        public FoodItemController(ICrudFoodItem crudFoodItem)
        {
            _crudFoodItem = crudFoodItem;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var foodItems = await _crudFoodItem.GetAllAsync();
            return Ok(foodItems);
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var foodItem = await _crudFoodItem.GetByIdAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }
            return Ok(foodItem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]FoodItem foodItem, string imageFileName)
        {
            foodItem.ImageURL = imageFileName;
            await _crudFoodItem.CreateAsync(foodItem); 
            return Ok(foodItem); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id,[FromForm] FoodItem foodItem, string imageFileName)
        {
            var exit= await _crudFoodItem.GetByIdAsync(id);
            if (exit == null)
            {
                return NotFound();
            }
            if (imageFileName != null && imageFileName.Length > 0)
            {
                await _crudFoodItem.UpdateAsync(id,foodItem, imageFileName);
            }
            else
            {
                return BadRequest("KHÔNG CÓ HÌNH ẢNH");
            }
            return Ok(foodItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exit = await _crudFoodItem.GetByIdAsync(id);
            if (exit == null)
            {
                return NotFound();
            }
            await _crudFoodItem.DeleteAsync(id);
            return NoContent();    
        }
    }
}
