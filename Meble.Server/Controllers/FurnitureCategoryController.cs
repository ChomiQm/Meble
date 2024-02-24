using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meble.Server.Models;

namespace Meble.Server.Controllers;

[ApiController]
[Route("furnitureCategories")]
public class FurnitureCategoryController : ControllerBase
{
    private readonly ILogger<FurnitureCategoryController> _logger;
    private readonly ModelContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public FurnitureCategoryController(ILogger<FurnitureCategoryController> logger,
                                       ModelContext context,
                                       UserManager<User> userManager,
                                       RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize]
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetFurnitureCategoriesDistinct()
    {
        var categories = await _context.FurnitureCategories
            .Select(c => c.CategoryName)
            .Distinct()
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("getCategories")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FurnitureCategory>>> GetFurnitureCategories()
    {
        return await _context.FurnitureCategories.ToListAsync();
    }

    [HttpGet("getCategory/{id}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FurnitureCategory>>> GetFurnitureCategory([FromRoute] int id)
    {
        var furnitureCategories = await _context.FurnitureCategories
            .Where(category => category.CategoryFurnitureId == id)
            .ToListAsync();

        if (!furnitureCategories.Any())
        {
            return NotFound();
        }

        return furnitureCategories;
    }

    [HttpPost("addCategory")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FurnitureCategory>> PostFurnitureCategory([FromBody]FurnitureCategory furnitureCategory)
    {
        _context.FurnitureCategories.Add(furnitureCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFurnitureCategory), new { id = furnitureCategory.CategoryId }, furnitureCategory);
    }

    [HttpPut("updateCategory/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutFurnitureCategory([FromRoute] int id, [FromBody] FurnitureCategory furnitureCategory)
    {
        if (id != furnitureCategory.CategoryId)
        {
            return BadRequest();
        }

        _context.Entry(furnitureCategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FurnitureCategoryExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("deleteCategory/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFurnitureCategory([FromRoute] int id)
    {
        var furnitureCategory = await _context.FurnitureCategories.FindAsync(id);
        if (furnitureCategory == null)
        {
            return NotFound();
        }

        _context.FurnitureCategories.Remove(furnitureCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FurnitureCategoryExists(int id)
    {
        return _context.FurnitureCategories.Any(e => e.CategoryId == id);
    }
}
