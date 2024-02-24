using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meble.Server.Models;
using Microsoft.AspNetCore.Identity;
using Meble.Server.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace Meble.Server.Controllers;

[ApiController]
[Route("account")]
public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ModelContext context,
    ILogger<AccountController> logger
    ) : Controller
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly ModelContext _context = context;
    private readonly ILogger<AccountController> _logger = logger;

    [Authorize]
    [HttpGet("currentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(new { error = "User not found." });
        }

        return Ok(new
        {
            email = user.Email,
            phoneNumber = user.PhoneNumber
        });
    }

    [SwaggerOperation(Summary = "Managing handler", Description = "Action allows a user to manage account.")]
    [HttpPut("manageUser")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] User model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { error = "User not found." });
                }

                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    return Unauthorized(new { error = "Unauthorized to update non-user role." });
                }

                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    var (isPasswordValid, passwordErrorMessage) = ValidationHelpers.IsPasswordValid(model.PasswordHash);
                    if (isPasswordValid)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, model.PasswordHash);
                        if (!result.Succeeded)
                        {
                            return BadRequest(new { error = "Error updating password." });
                        }
                    }
                    else
                    {
                        return BadRequest(new { error = passwordErrorMessage });
                    }
                }

                user.UserDateOfUpdate = DateTime.Now;

                // Update Email
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var (isEmailValid, emailErrorMessage) = ValidationHelpers.IsEmailValid(model.Email);
                    if (isEmailValid)
                    {
                        user.Email = model.Email;
                    }
                    else
                    {
                        return BadRequest(new { error = emailErrorMessage });
                    }
                }

                // Update PhoneNumber
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var (isPhoneValid, phoneErrorMessage) = ValidationHelpers.IsPhoneNumberValid(model.PhoneNumber);
                    if (isPhoneValid)
                    {
                        user.PhoneNumber = model.PhoneNumber;
                    }
                    else
                    {
                        return BadRequest(new { error = phoneErrorMessage });
                    }
                }

                var resultUpdate = await _userManager.UpdateAsync(user);
                if (resultUpdate.Succeeded)
                {
                    return Ok(new { message = "User updated successfully." });
                }
                else
                {
                    return BadRequest(new { error = "An error occurred during updating data." });
                }
            }
            else
            {
                return BadRequest(new { error = "Invalid model data." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during updating data.");
            return BadRequest(new { error = "An error occurred during updating data." });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "User logged out successfully." });
    }

    [HttpDelete("delete")]
    [Authorize]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { error = "User not found." });
            }

            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                return Unauthorized(new { error = "Unauthorized to delete non-user role." });
            }

            // Pobranie danych użytkownika
            var userData = await _context.UserDatas.FirstOrDefaultAsync(ud => ud.UserId == user.Id);
            if (userData != null)
            {
                // Usuwanie danych użytkownika
                _context.UserDatas.Remove(userData);
                await _context.SaveChangesAsync();
            }

            // Usuwanie samego użytkownika
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return Ok(new { message = "User deleted successfully." });
            }
            else
            {
                return BadRequest(new { error = "An error occurred during removal." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during removal.");
            return BadRequest(new { error = "An error occurred during removal." });
        }
    }

    [HttpPost("validatePassword")]
    [Authorize]
    public async Task<IActionResult> ValidatePassword([FromBody] PasswordValidationModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest(new { error = "Password is required." });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(new { error = "User not found." });
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            return Unauthorized(new { error = "Invalid password." });
        }

        return Ok(new { message = "Password is valid." });
    }


}
public class PasswordValidationModel
{
    public string? Password { get; set; }
}