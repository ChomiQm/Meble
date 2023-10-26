using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Meble.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Meble.Server.Controllers
{
    [ApiController]
    [Route("info")]
    public class InfoController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<InfoController> _logger;
        private readonly ModelContext _context;

        public InfoController(
            ILogger<InfoController> logger,
            ModelContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserData>>> GetUserDatas()
        {
            return await _context.Set<UserData>().ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<UserData>> PostUserData(UserData userData)
        {
            var id = new Guid().ToString();
            userData.UserDataId = id;

            if (userData == null)
            {
                return BadRequest(new { error = "Provided user data is null." });
            }

            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (userData.User != null)
            {
                userData.User.UserDataId = userData.UserDataId;
            }

            _context.Set<UserData>().Add(userData);
            await _context.SaveChangesAsync();

            // add current user to a role after saving the user data
            var result = await _userManager.AddToRoleAsync(currentUser, "User");
            if (!result.Succeeded)
            {
                _logger.LogError($"Error adding user with ID {currentUser.Id} to role {"User"}: {string.Join(", ", result.Errors)}");
            }

            return CreatedAtAction(nameof(GetUserDataById), new { id = userData.UserDataId }, userData);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserData>> GetUserDataById(string id)
        {
            var userData = await _context.Set<UserData>().FindAsync(id);

            if (userData == null)
            {
                return NotFound();
            }

            return userData;
        }
    }
}
