using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meble.Server.Models;

namespace Meble.Server.Controllers;

[ApiController]
[Route("furnitures")]

public class FurnitureController : ControllerBase
{
    private readonly ILogger<FurnitureController> _logger;
    private readonly ModelContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public FurnitureController(ILogger<FurnitureController> logger,
                               ModelContext context,
                               UserManager<User> userManager,
                               RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("getAllFurnitures")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Furniture>>> GetFurnitures()
    {
        return await _context.Furnitures.ToListAsync();
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Furniture>>> SearchFurnitures([FromQuery] string? name, [FromQuery] string? category)
    {
        var query = _context.Furnitures.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(f => f.FurnitureName.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(f => f.FurnitureCategories.Any(fc => fc.CategoryName == category));
        }

        var result = await query
            .Include(f => f.FurnitureCategories)
            .Include(f => f.FurniturePhotos)
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("getFurniture/{id}")]
    [Authorize]
    public async Task<ActionResult<Furniture>> GetFurniture([FromRoute] int id)
    {
        var furniture = await _context.Furnitures.FindAsync(id);

        if (furniture == null)
        {
            return NotFound();
        }

        return furniture;
    }

    [HttpPost("addFurniture")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Furniture>> PostFurniture([FromBody] Furniture furniture)
    {
        _context.Furnitures.Add(furniture);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFurniture), new { id = furniture.FurnitureId }, furniture);
    }

    [HttpPut("updateFurniture/{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> PutFurniture([FromRoute] int id, [FromBody] Furniture furniture)
    {
        if (id != furniture.FurnitureId)
        {
            return BadRequest();
        }

        _context.Entry(furniture).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FurnitureExists(id))
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

    [HttpDelete("deleteFurniture/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFurniture([FromRoute] int id)
    {
        var furniture = await _context.Furnitures.FindAsync(id);
        if (furniture == null)
        {
            return NotFound();
        }

        _context.Furnitures.Remove(furniture);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FurnitureExists(int id)
    {
        return _context.Furnitures.Any(e => e.FurnitureId == id);
    }
}
