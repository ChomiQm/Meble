//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Meble.Models;
//using Microsoft.AspNetCore.Authorization;
//using Meble.Helpers;

//namespace Meble.Controllers.AdminControllers
//{
//    [Authorize(Roles = "Admin")]
//    [Route("api/furniture")]
//    public class FurnitureController : Controller
//    {
//        private readonly ModelContext _context;
//        private readonly ILogger<FurnitureController> _logger;

//        public FurnitureController(ModelContext context, ILogger<FurnitureController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        [HttpGet("{furnitureId}")]
//        public async Task<IActionResult> GetFurniture(int furnitureId)
//        {
//            var furniture = await _context.Furnitures.SingleOrDefaultAsync(f => f.FurnitureId == furnitureId);

//            if (furniture == null)
//            {
//                return NotFound();
//            }

//            return Ok(furniture);
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPost("add_furniture")]
//        public async Task<IActionResult> CreateFurniture(Furniture furniture)
//        {
//            try
//            {
//                // validate
//                if (!ValidationHelpers.IsPriceValid(furniture.FurniturePrice))
//                {
//                    ModelState.AddModelError(string.Empty, "Invalid price");
//                    return BadRequest(ModelState);
//                }

//                if (furniture.FurnitureCategories.Any(category => string.IsNullOrEmpty(category.FurnitureCategoryName)))
//                {
//                    ModelState.AddModelError(string.Empty, "Category name cannot be empty");
//                    return BadRequest(ModelState);
//                }

//                furniture.FurnitureDateOfAddition = DateTime.Now;

//                // add furniture
//                _context.Furnitures.Add(furniture);
//                await _context.SaveChangesAsync();

//                return CreatedAtAction("GetFurniture", new { furnitureId = furniture.FurnitureId }, furniture);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error during furniture creation");
//                return StatusCode(500);
//            }
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPut("{furnitureId}")]
//        public async Task<IActionResult> UpdateFurniture(int furnitureId, Furniture updatedFurniture, FurnitureCategory updatedCategory)
//        {
//            var furniture = await _context.Furnitures
//                .Include(f => f.FurnitureCategories)
//                .SingleOrDefaultAsync(f => f.FurnitureId == furnitureId);

//            if (furniture == null)
//            {
//                return NotFound();
//            }

//            // validation
//            if (!ValidationHelpers.IsPriceValid(updatedFurniture.FurniturePrice))
//            {
//                ModelState.AddModelError(string.Empty, "Invalid price");
//                return BadRequest(ModelState);
//            }

//            if (!ValidationHelpers.IsEmailValid(updatedFurniture.FurnitureManufacturer.ManufacturerMail))
//            {
//                ModelState.AddModelError(string.Empty, "Invalid manufacturer email address");
//                return BadRequest(ModelState);
//            }

//            if (updatedFurniture.FurnitureCategories.Any(category => string.IsNullOrEmpty(category.FurnitureCategoryName)))
//            {
//                ModelState.AddModelError(string.Empty, "Category name cannot be empty");
//                return BadRequest(ModelState);
//            }

//            // furniture update
//            furniture.FurnitureName = updatedFurniture.FurnitureName;
//            furniture.FurniturePrice = updatedFurniture.FurniturePrice;
//            furniture.FurnitureDescription = updatedFurniture.FurnitureDescription;
//            furniture.FurnitureManufacturer = updatedFurniture.FurnitureManufacturer;

//            // update category which is connected to furniture by id
//            var category = furniture.FurnitureCategories.SingleOrDefault(c => c.FurnitureCategoryId == updatedCategory.FurnitureCategoryId);
//            if (category != null)
//            {
//                category.FurnitureCategoryName = updatedCategory.FurnitureCategoryName;
//            }

//            furniture.FurnitureDateOfUpdate = DateTime.Now;

//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpDelete("{furnitureId}")]
//        public async Task<IActionResult> DeleteFurniture(int furnitureId)
//        {
//            var furniture = await _context.Furnitures.FindAsync(furnitureId);

//            if (furniture == null)
//            {
//                return NotFound();
//            }

//            _context.Furnitures.Remove(furniture);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [HttpGet("{categoryId}")]
//        public async Task<IActionResult> GetFurnitureCategory(int categoryId)
//        {
//            var category = await _context.FurnitureCategories.SingleOrDefaultAsync(c => c.FurnitureCategoryId == categoryId);

//            if (category == null)
//            {
//                return NotFound();
//            }

//            return Ok(category);
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPost("add_category/{furnitureId}")]
//        public async Task<IActionResult> AddCategoryToFurniture(int furnitureId, FurnitureCategory category)
//        {
//            try
//            {
//                var furniture = await _context.Furnitures.SingleOrDefaultAsync(f => f.FurnitureId == furnitureId);

//                if (furniture == null)
//                {
//                    return NotFound("Furniture not found");
//                }

//                if (string.IsNullOrEmpty(category.FurnitureCategoryName))
//                {
//                    ModelState.AddModelError(string.Empty, "Category name cannot be empty");
//                    return BadRequest(ModelState);
//                }

//                // add category to furniture
//                furniture.FurnitureCategories.Add(category);

//                await _context.SaveChangesAsync();

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error during adding category to furniture");
//                return StatusCode(500);
//            }
//        }


//        [Authorize(Roles = "Admin")]
//        [HttpPut("{categoryId}")]
//        public async Task<IActionResult> UpdateFurnitureCategory(int categoryId, FurnitureCategory updatedCategory)
//        {
//            var category = await _context.FurnitureCategories.FindAsync(categoryId);

//            if (category == null)
//            {
//                return NotFound();
//            }

//            if (string.IsNullOrEmpty(updatedCategory.FurnitureCategoryName))
//            {
//                ModelState.AddModelError(string.Empty, "Category name cannot be empty");
//                return BadRequest(ModelState);
//            }
//            category.FurnitureCategoryName = updatedCategory.FurnitureCategoryName;

//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpDelete("{categoryId}")]
//        public async Task<IActionResult> DeleteFurnitureCategory(int categoryId)
//        {
//            var category = await _context.FurnitureCategories.FindAsync(categoryId);

//            if (category == null)
//            {
//                return NotFound();
//            }

//            _context.FurnitureCategories.Remove(category);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }


//    }
//}
