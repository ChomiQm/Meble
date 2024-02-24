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

    [Authorize(Roles = "Admin")]
    [HttpGet("getAllUserOrders")]
    public async Task<ActionResult<IEnumerable<ClientOrderDetails>>> GetAllOrders()
    {
        var orders = await _context.ClientOrders
            .Include(o => o.OrderFurnitures)
                .ThenInclude(of => of.Furniture)
            .Include(o => o.OrderUser)
                .ThenInclude(u => u.UserDatas) 
            .ToListAsync();

        var orderDetailsList = orders.Select(order => new ClientOrderDetails
        {
            OrderId = order.OrderId,
            OrderDateOfOrder = order.OrderDateOfOrder,
            Furnitures = order.OrderFurnitures.Select(of => of.Furniture).ToList(),
            QuantityOrdered = order.OrderFurnitures.Select(of => of.QuantityOrdered).ToList(),
            TotalOrderValue = order.TotalOrderValue,
            TotalItemsOrdered = order.TotalItemsOrdered,
            OrderStatus = order.OrderStatus,
            UserFirstName = order.OrderUser?.UserDatas?.UserFirstName ?? "",
            UserSurname = order.OrderUser?.UserDatas?.UserSurname ?? "",
            UserCountry = order.OrderUser?.UserDatas?.UserCountry ?? "",
            UserTown = order.OrderUser?.UserDatas?.UserTown ?? "",
            UserStreet = order.OrderUser?.UserDatas?.UserStreet,
            UserHomeNumber = order.OrderUser?.UserDatas?.UserHomeNumber,
            UserFlatNumber = order.OrderUser?.UserDatas?.UserFlatNumber
        }).ToList();
        
        return orderDetailsList;
    }

    [Authorize]
    [HttpGet("userOrders")]
    public async Task<ActionResult<IEnumerable<ClientOrderDetails>>> GetUserOrders()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return BadRequest("Cannot find the current user.");
        }

        var orders = await _context.ClientOrders
            .Where(o => o.OrderUserId == currentUser.Id)
            .Include(o => o.OrderFurnitures)
            .ThenInclude(of => of.Furniture)
            .ToListAsync();

        var orderDetailsList = orders.Select(order => new ClientOrderDetails
        {
            OrderId = order.OrderId,
            OrderDateOfOrder = order.OrderDateOfOrder,
            Furnitures = order.OrderFurnitures.Select(of => of.Furniture).ToList(),
            TotalOrderValue = order.TotalOrderValue,
            TotalItemsOrdered = order.TotalItemsOrdered,
            QuantityOrdered = order.OrderFurnitures.Select(of => of.QuantityOrdered).ToList(),
            OrderStatus = order.OrderStatus
        }).ToList();

        return orderDetailsList;
    }


    [Authorize]
    [HttpPost("createOrder")]
    public async Task<ActionResult<ClientOrder>> CreateOrder([FromBody]CreateOrderRequest request)
    {
        var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return BadRequest("Cannot find the current user.");
            }

            var furnitureList = await _context.Furnitures
                .Where(f => request.FurnitureIds.Contains(f.FurnitureId) && f.IsAvailable)
                .ToListAsync();        

            if (furnitureList.Count != request.FurnitureIds.Count)
            {
                var notFoundFurnitureIds = request.FurnitureIds.Except(furnitureList.Select(f => f.FurnitureId));
                return BadRequest($"Some furniture items were not found or are unavailable: {string.Join(", ", notFoundFurnitureIds)}");
            }

            var order = new ClientOrder
            {
                OrderUserId = currentUser.Id,
                OrderDateOfOrder = DateOnly.FromDateTime(DateTime.Now),
                TotalOrderValue = request.TotalOrderValue,
                TotalItemsOrdered = request.TotalItemsOrdered,
                OrderStatus = "PENDING"
            };

            _context.ClientOrders.Add(order);
            await _context.SaveChangesAsync();

            for(int i = 0; i < furnitureList.Count; i++){
                var furniture = furnitureList[i];
                var orderFurniture = new OrderFurniture
                {
                    OrderId = order.OrderId,
                    FurnitureId = furniture.FurnitureId,
                    QuantityOrdered = request.QuantityOrdered[i]
                };

                _context.OrderFurnitures.Add(orderFurniture);
            }

            await _context.SaveChangesAsync();

            var totalPrice = furnitureList.Sum(f => f.FurniturePrice);
            await transaction.CommitAsync();

            var orderDetails = new
            {
                OrderId = order.OrderId,
                TotalPrice = totalPrice,
                Furnitures = furnitureList
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, orderDetails);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest($"An error occurred while creating the order: {ex.Message}");
        }
    }

    [Authorize]
    [HttpGet("getOrder/{id}")]
    public async Task<ActionResult<ClientOrderDetails>> GetOrder([FromRoute] int id)
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
            Furnitures = order.OrderFurnitures.Select(of => of.Furniture).ToList(),
            TotalOrderValue = order.TotalOrderValue,
            TotalItemsOrdered = order.TotalItemsOrdered
        };

        return details;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("updateOrderStatus/{orderId}")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _context.ClientOrders.FindAsync(orderId);

        if (order == null)
        {
            return NotFound($"Order with ID {orderId} not found.");
        }

        order.OrderStatus = request.OrderStatus;
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteOrder/{id}")]
    public async Task<IActionResult> DeleteOrder([FromRoute] int id)
    {
        var order = await _context.ClientOrders
            .Include(o => o.OrderFurnitures)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        _context.ClientOrders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content response
    }

}

public class CreateOrderRequest
{
    public List<int> FurnitureIds { get; set; } = new();
    public decimal TotalOrderValue { get; set; } 
    public int TotalItemsOrdered { get; set; } 
    public List<int> QuantityOrdered { get; set; }
}


public class ClientOrderDetails
{
    public int OrderId { get; set; }
    public DateOnly? OrderDateOfOrder { get; set; }
    public List<Furniture> Furnitures { get; set; } = new();
    public List<int?> QuantityOrdered {get; set;} = new();
    public decimal TotalOrderValue { get; set; }
    public int TotalItemsOrdered { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string UserFirstName { get; set; } = null!;
    public string UserSurname { get; set; } = null!;
    public string UserCountry { get; set; } = null!;
    public string UserTown { get; set; } = null!;
    public string? UserStreet { get; set; }
    public int? UserHomeNumber { get; set; }
    public string? UserFlatNumber { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string OrderStatus { get; set; } = null!;
}