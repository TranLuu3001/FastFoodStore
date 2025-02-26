using FastFoodStore_API.Interfaces;
using FastFoodStore_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFoodStore_API.Services
{
    public class CrudComboService:ICrudCombo
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public CrudComboService(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<Combo>> GetAllAsync()
        {
            return await _context.Combos.ToListAsync();
        }

        public async Task<Combo?> GetByIdAsync(int id)
        {
            return await _context.Combos.FindAsync(id);
        }

        public async Task CreateAsync(Combo combo)
        {
            combo.CreatedAt = DateTime.Now;
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Combo combo, string imageFile, int id)
        {
            var existingItem = await _context.Combos.FindAsync(id);
            if (existingItem != null)
            {
                existingItem.ComboName = combo.ComboName;
                existingItem.Description = combo.Description;
                existingItem.Price = combo.Price;
                existingItem.ImageURL = imageFile;
                existingItem.CreatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo != null)
            {
                if (!string.IsNullOrEmpty(combo.ImageURL))
                {
                    DeleteImage(combo.ImageURL);
                }

                _context.Combos.Remove(combo);
                await _context.SaveChangesAsync();
            }
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


        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                // Xử lý trường hợp imageUrl null hoặc rỗng, ví dụ log lỗi hoặc return
                Console.WriteLine("Image URL is null or empty.");
                return;
            }

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageUrl);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
                Console.WriteLine("Image deleted successfully.");
            }
            else
            {
                Console.WriteLine("Image file does not exist.");
            }
        }
    }
}
