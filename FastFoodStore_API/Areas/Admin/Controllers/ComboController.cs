using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodStore_API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    public class ComboController : Controller
    {
        private readonly ICrudCombo _crudCombo;

        public ComboController(ICrudCombo crudCombo)
        {
            _crudCombo = crudCombo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var combo = await _crudCombo.GetAllAsync();
            return Ok(combo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var combo = await _crudCombo.GetByIdAsync(id);
            if (combo == null)
            {
                return NotFound();
            }
            return Ok(combo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Combo combo, string imageFileName)
        {
            combo.ImageURL = imageFileName;
            await _crudCombo.CreateAsync(combo);
            return Ok(combo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Combo combo, string imageFileName)
        {
            var exit = await _crudCombo.GetByIdAsync(id);
            if (exit == null)
            {
                return NotFound();
            }
            if (imageFileName != null && imageFileName.Length > 0)
            {
                await _crudCombo.UpdateAsync(combo, imageFileName, id);
            }
            else
            {
                return BadRequest("KHÔNG CÓ HÌNH ẢNH");
            }
            return Ok(combo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exit = await _crudCombo.GetByIdAsync(id);
            if (exit == null)
            {
                return NotFound();
            }
            await _crudCombo.DeleteAsync(id);
            return NoContent();
        }
    }
}
