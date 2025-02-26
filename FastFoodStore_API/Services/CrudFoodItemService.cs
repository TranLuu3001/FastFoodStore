using FastFoodStore_API.Models;
using FastFoodStore_API.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FastFoodStore_API.Services
{
    public class CrudFoodItemService : ICrudFoodItem
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public CrudFoodItemService(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<FoodItem>> GetAllAsync()
        {
            return await _context.FoodItems.ToListAsync();
        }

        public async Task<FoodItem?> GetByIdAsync(int id)
        {
            return await _context.FoodItems.FindAsync(id);
        }

        public async Task CreateAsync(FoodItem foodItem)
        {
            //if (imageFile != null && imageFile.Length > 0)
            //{
                
            //    string imagePath = await SaveImageAsync(imageFile);
            //    foodItem.ImageURL = imagePath;
            //}
            foodItem.CreatedAt = DateTime.Now;
            _context.FoodItems.Add(foodItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, FoodItem foodItem, string image)
        {
            var existingItem = await _context.FoodItems.FindAsync(id);
            if (existingItem != null)
            {
                existingItem.FoodName = foodItem.FoodName;
                existingItem.Category = foodItem.Category;
                existingItem.Price = foodItem.Price;
                existingItem.Description = foodItem.Description;
                existingItem.ImageURL = image;  

                //if (imageFile != null && imageFile.Length > 0)
                //{
                //    if (!string.IsNullOrEmpty(existingItem.ImageURL))
                //    {
                //        DeleteImage(existingItem.ImageURL);
                //    }
                //    string imagePath = await SaveImageAsync(imageFile);
                //    existingItem.ImageURL = imagePath;
                //}

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem != null)
            {
                if (!string.IsNullOrEmpty(foodItem.ImageURL))
                {
                    DeleteImage(foodItem.ImageURL);
                }

                _context.FoodItems.Remove(foodItem);
                await _context.SaveChangesAsync();
            }
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



        public async Task<List<CartItem>> GetCartItemAsync(string userId)
        {
            //string query = @"
            //SELECT dbo.FoodItems.FoodName, dbo.FoodItems.Price, dbo.Cart.Quantity, dbo.FoodItems.ImageURL, dbo.Cart.UserID
            //FROM dbo.Cart
            //INNER JOIN dbo.FoodItems ON dbo.Cart.FoodItemId = dbo.FoodItems.FoodId
            //WHERE dbo.Cart.UserID = {0}";

            var carts = await _context.Cart
                                      .Where(c=>c.UserID==userId).Include(c=>c.FoodItem)
                                      .Select(c => new CartItem
                                      {
                                          CartId = c.CartId,
                                          FoodName = c.FoodItem!.FoodName,
                                          Price = c.FoodItem.Price,
                                          Quatity = c.Quatity,
                                          FoodItemId = c.FoodItemId,
                                          UserId = c.UserID,
                                          ImageUrl = c.FoodItem.ImageURL
                                      }).ToListAsync();
            return carts;
        }

        //private async Task<decimal> GetPriceToCartAsync(int foodItemId)
        //{
        //    var fooditem = await _context.FoodItems.FindAsync(foodItemId);
        //    var cart = await _context.Cart.Where(c=>c.CartId==fooditem.)
        //    return fooditem.Price;
        //}
        //private async Task<string> GetImageFoodToCartAsync(int foodItemId)
        //{
        //    var fooditem = await _context.FoodItems.FindAsync(foodItemId);
        //    return fooditem.ImageURL;
        //}
        //private async Task<string> GetFoodNameToCartAsync(int foodItemId)
        //{
        //    var fooditem = await _context.FoodItems.FindAsync(foodItemId);
        //    return fooditem.FoodName;
        //}
        public async Task<bool> AddToCartAsync(string userId, int foodItemId, int quantity)
        {
            try
            {
                var cart = await _context.Cart
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.FoodItemId == foodItemId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserID = userId,
                        FoodItemId = foodItemId,
                        Quatity = quantity,
                        Total = 0
                    };
                    _context.Cart.Add(cart);
                }
                else
                {
                    cart.Quatity += quantity;
                }

                var foodItem = await _context.FoodItems
                    .FirstOrDefaultAsync(f => f.FoodId == foodItemId);

                if (foodItem == null)
                {
                    throw new Exception("FoodItem is null.");
                }

                cart.Total = foodItem.Price * cart.Quatity;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi và hiển thị thông báo chi tiết hơn
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteFoodInCart(string userId, int foodItemId, int quantity)
        {
            var cart = await _context.Cart.Where(c=>c.UserID == userId && c.FoodItemId == foodItemId).ToListAsync();
            if (!cart.Any())
            {
                return false;
            }
            foreach (var x in cart)
            {
             _context.Cart.Remove(x); 
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
