using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meble.Server.Helpers;
using Meble.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Meble.Server.Controllers
{
    [ApiController]
    [Route("info")]
    public class InfoController(
        ILogger<InfoController> logger,
        ModelContext context,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager) : ControllerBase
        
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly ILogger<InfoController> _logger = logger;
        private readonly ModelContext _context = context;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

       
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserData>>> GetUserDatas()
        {
            return await _context.Set<UserData>().ToListAsync();
        }

        [HttpPost("addInfo")]
        [Authorize]
        public async Task<ActionResult<UserData>> PostUserData([FromBody] UserData userData)
        {
            if (userData == null)
            {
                return BadRequest(new { error = "Provided user data is null." });
            }

            // Generate a new unique identifier for UserDataId
            userData.UserDataId = Guid.NewGuid().ToString();

            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            userData.UserId = currentUser.Id;

            if (!string.IsNullOrEmpty(userData.UserCountry))
            {
                var (isCountryValid, countryErrorMessage) = ValidationHelpers.IsCountryValid(userData.UserCountry);
                if (!isCountryValid)
                {
                    return BadRequest(new { error = countryErrorMessage });
                }
            }

            if (!string.IsNullOrEmpty(userData.UserStreet))
            {
                var (isStreetValid, firstNameErrorMessage) = ValidationHelpers.IsStreetValid(userData.UserStreet);
                if (!isStreetValid)
                {
                    return BadRequest(new { error = firstNameErrorMessage });
                }
            }

            if (!string.IsNullOrEmpty(userData.UserFirstName))
            {
                var (isFirstNameValid, firstNameErrorMessage) = ValidationHelpers.IsNameValid(userData.UserFirstName);
                if (!isFirstNameValid)
                {
                    return BadRequest(new { error = firstNameErrorMessage });
                }
            }

            if (!string.IsNullOrEmpty(userData.UserSurname))
            {
                var (isSurnameValid, surnameErrorMessage) = ValidationHelpers.IsNameValid(userData.UserSurname);
                if (!isSurnameValid)
                {
                    return BadRequest(new { error = surnameErrorMessage });
                }
            }

            if (!string.IsNullOrEmpty(userData.UserTown))
            {
                var (isTownValid, townErrorMessage) = ValidationHelpers.IsTownValid(userData.UserTown);
                if (!isTownValid)
                {
                    return BadRequest(new { error = townErrorMessage });
                }
            }

            if (!string.IsNullOrEmpty(userData.UserFlatNumber))
            {
                var (isFlatNumberValid, flatNumberErrorMessage) = ValidationHelpers.IsFLatNumberValid(userData.UserFlatNumber);
                if (!isFlatNumberValid)
                {
                    return BadRequest(new { error = flatNumberErrorMessage });
                }
            }

            try
            {
                // Add the new userData to the database context
                _context.Set<UserData>().Add(userData);
                await _context.SaveChangesAsync();

                // Ensure the 'User' role exists
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError($"Error creating role 'User': {string.Join(", ", roleResult.Errors)}");
                        return StatusCode(500, new { error = $"Could not create the 'User' role: {string.Join(", ", roleResult.Errors)}" });
                    }
                }

                // Add the current user to the 'User' role
                var addToRoleResult = await _userManager.AddToRoleAsync(currentUser, "User");
                if (!addToRoleResult.Succeeded)
                {
                    _logger.LogError($"Error adding user with ID {currentUser.Id} to role 'User': {string.Join(", ", addToRoleResult.Errors)}");
                    return StatusCode(500, new { error = $"Could not add user to 'User' role: {string.Join(", ", addToRoleResult.Errors)}" });
                }

                return CreatedAtAction(nameof(GetActualUserData), new { id = userData.UserDataId }, userData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when saving the data: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred when saving the data." });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData( [FromBody] UserData userDataUpdate)
        {
            var userId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var userData = await _context.UserDatas
                                        .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userDataUpdate == null)
            {
                return BadRequest(new { error = "Dostarczone dane użytkownika są puste." });
            }

            if (userData == null)
            {
                return NotFound();
            }

            // Weryfikacja, czy zalogowany użytkownik może zaktualizować te dane
            if (userId == null || currentUser == null || userData.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            var (isFirstNameValid, firstNameErrorMessage) = ValidationHelpers.IsNameValid(userDataUpdate.UserFirstName);
            if (!isFirstNameValid)
            {
                return BadRequest(new { error = firstNameErrorMessage });
            }

            var (isSurnameValid, surnameErrorMessage) = ValidationHelpers.IsNameValid(userDataUpdate.UserSurname);
            if (!isSurnameValid)
            {
                return BadRequest(new { error = surnameErrorMessage });
            }

            var (isCountryValid, countryErrorMessage) = ValidationHelpers.IsCountryValid(userDataUpdate.UserCountry);
            if (!isCountryValid)
            {
                return BadRequest(new { error = countryErrorMessage });
            }

            var (isTownValid, townErrorMessage) = ValidationHelpers.IsTownValid(userDataUpdate.UserTown);
            if (!isTownValid)
            {
                return BadRequest(new { error = townErrorMessage });
            }

            // Aktualizacja właściwości
            userData.UserFirstName = userDataUpdate.UserFirstName;
            userData.UserSurname = userDataUpdate.UserSurname;
            userData.UserCountry = userDataUpdate.UserCountry;
            userData.UserTown = userDataUpdate.UserTown;
            userData.UserFlatNumber = userDataUpdate.UserFlatNumber;

            _context.Entry(userData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDataExists(userId))
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

        [HttpPost("hasUserData")]
        [Authorize]
        public async Task<ActionResult<bool>> HasUserData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var userData = await _context.Set<UserData>().FirstOrDefaultAsync(ud => ud.User == currentUser);
            return userData != null;
        }

        [HttpGet("getData")]
        [Authorize]
        public async Task<ActionResult<UserData>> GetActualUserData()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, "Unauthorized");
            }

            var userData = await _context.UserDatas
                                         .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userData == null)
            {
                return StatusCode(500, "Internal server error");
            }

            return userData;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Pobranie identyfikatora użytkownika z tokenu
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Pobranie użytkownika
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Pobranie ról użytkownika
            var roles = await _userManager.GetRolesAsync(user);

            // Tworzenie obiektu DTO (Data Transfer Object) z danymi użytkownika
            var userInfo = new
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles
            };

            return Ok(userInfo);
        }

        private bool UserDataExists(string id)
        {
            return _context.Set<UserData>().Any(e => e.UserDataId == id);
        }

    }
}
