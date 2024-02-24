using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meble.Server.Models;
using Meble.Server.Services;

namespace Meble.Server.Controllers;

[ApiController]
[Route("photoFurniture")]
public class FurniturePhotoController : ControllerBase
{
    private readonly ILogger<FurniturePhotoController> _logger;
    private readonly ModelContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public FurniturePhotoController(ILogger<FurniturePhotoController> logger,
                                    ModelContext context,
                                    UserManager<User> userManager,
                                    RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("getAllPhotos")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FurniturePhoto>>> GetFurniturePhotos()
    {
        return await _context.FurniturePhotos.ToListAsync();
    }

    [HttpGet("getPhoto/{id}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FurniturePhoto>>> GetFurniturePhoto([FromRoute] int id)
    {
        var furniturePhotos = await _context.FurniturePhotos
            .Where(photo => photo.PhotoFurnitureId == id)
            .ToListAsync();

        if (!furniturePhotos.Any())
        {
            return NotFound();
        }

        return furniturePhotos;
    }
    [HttpPost("addPhoto/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FurniturePhoto>> PostFurniturePhoto([FromRoute] int id, [FromForm] IFormFile file, [FromForm] string photoDescription)
    {
        var azureBlobService = HttpContext.RequestServices.GetService<AzureBlobService>();

        using var stream = file.OpenReadStream();
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var photoUrl = await azureBlobService.UploadFileAsync(stream, fileName);

        var furniturePhoto = new FurniturePhoto
        {
            PhotoFurnitureId = id,
            PhotoUrl = photoUrl,
            PhotoDescription = photoDescription  // Dodano właściwość PhotoDescription
        };

        _context.FurniturePhotos.Add(furniturePhoto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFurniturePhoto), new { id = furniturePhoto.PhotoId }, furniturePhoto);
    }


    [HttpPut("updatePhoto/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutFurniturePhoto([FromRoute]int id,[FromBody] FurniturePhoto furniturePhoto)
    {
        if (id != furniturePhoto.PhotoId)
        {
            return BadRequest();
        }

        _context.Entry(furniturePhoto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FurniturePhotoExists(id))
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

    [HttpDelete("deletePhoto/{photoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFurniturePhoto([FromRoute] int photoId)
    {
        var azureBlobService = HttpContext.RequestServices.GetService<AzureBlobService>();
        var furniturePhoto = await _context.FurniturePhotos.FindAsync(photoId);

        if (furniturePhoto == null)
        {
            return NotFound();
        }

        // Usunięcie zdjęcia z Azure Blob Storage
        var fileName = GetFileNameFromUrl(furniturePhoto.PhotoUrl);
        await azureBlobService.DeleteFileAsync(fileName);

        // Usunięcie zdjęcia z bazy danych
        _context.FurniturePhotos.Remove(furniturePhoto);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("deletePhotosOfFurniture/{furnitureId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAllFurniturePhotos([FromRoute] int furnitureId)
    {
        var azureBlobService = HttpContext.RequestServices.GetService<AzureBlobService>();

        var furniturePhotos = await _context.FurniturePhotos
            .Where(p => p.PhotoFurnitureId == furnitureId)
            .ToListAsync();

        if (!furniturePhotos.Any())
        {
            return NotFound();
        }

        foreach (var furniturePhoto in furniturePhotos)
        {
            // Usunięcie zdjęcia z Azure Blob Storage
            var fileName = GetFileNameFromUrl(furniturePhoto.PhotoUrl);
            await azureBlobService.DeleteFileAsync(fileName);

            // Usunięcie zdjęcia z bazy danych
            _context.FurniturePhotos.Remove(furniturePhoto);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private string GetFileNameFromUrl(string url)
    {
        var uri = new Uri(url);
        return Path.GetFileName(uri.LocalPath);
    }

    private bool FurniturePhotoExists(int id)
    {
        return _context.FurniturePhotos.Any(e => e.PhotoId == id);
    }
}
