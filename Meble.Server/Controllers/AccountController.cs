using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meble.Server.Models;
using Microsoft.AspNetCore.Identity;
using Meble.Server.Helpers;
using Swashbuckle.AspNetCore.Annotations;
namespace Meble.Server.Controllers;

[ApiController]
[Route("account")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<AccountController> logger
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [SwaggerOperation(Summary = "Managing handler", Description = "Action allows a user to manage account.")]
    [HttpPut("manage")]
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

                if (model.UserDatas != null && user.UserDatas != null)
                {
                    user.UserDatas.UserFirstName = model.UserDatas.UserFirstName ?? user.UserDatas.UserFirstName;
                    user.UserDatas.UserSurname = model.UserDatas.UserSurname ?? user.UserDatas.UserSurname;
                }
                else
                {
                    return BadRequest(new { error = "User data is missing." });
                }

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
}
