using Meble.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("order")]
public class ClientOrderController : ControllerBase
{
    private readonly ModelContext _context;
    private readonly UserManager<User> _userManager;
    public ClientOrderController(ModelContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<ActionResult<ClientOrder>> CreateOrder(CreateOrderRequest request)
    {
        // Pobierz aktualnie zalogowanego użytkownika za pomocą UserManager
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            // Jeśli z jakiegoś powodu nie możemy znaleźć użytkownika, zwróć odpowiedni komunikat
            return BadRequest("Cannot find the current user.");
        }

        var order = new ClientOrder
        {
            OrderUserId = currentUser.Id, // Ustaw identyfikator użytkownika na identyfikator aktualnie zalogowanego użytkownika
            OrderDateOfOrder = DateOnly.FromDateTime(DateTime.Now)
        };

        _context.ClientOrders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var furnitureId in request.FurnitureIds)
        {
            var orderFurniture = new OrderFurniture
            {
                OrderId = order.OrderId,
                FurnitureId = furnitureId
            };

            _context.OrderFurnitures.Add(orderFurniture);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientOrderDetails>> GetOrder(int id)
    {
        var order = await _context.ClientOrders
            .Include(o => o.OrderFurnitures)
                .ThenInclude(of => of.Furniture)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        var details = new ClientOrderDetails
        {
            OrderId = order.OrderId,
            OrderDateOfOrder = order.OrderDateOfOrder,
            Furnitures = order.OrderFurnitures.Select(of => of.Furniture).ToList()
        };

        return details;
    }
}

public class CreateOrderRequest
{
    public string OrderUserId { get; set; }
    public List<int> FurnitureIds { get; set; } = new();
}

public class ClientOrderDetails
{
    public int OrderId { get; set; }
    public DateOnly? OrderDateOfOrder { get; set; }
    public List<Furniture> Furnitures { get; set; } = new();
}

