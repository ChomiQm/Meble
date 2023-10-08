using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using inzynierka_geska.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

[Route("account")]
public class AccountController : Controller
{
    private readonly ModelContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ModelContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(User model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Find user by login
                var user = await _context.Users
                    .Include(u => u.UserPerson) // Person addition
                    .SingleOrDefaultAsync(u => u.UserLogin == model.UserLogin);

                //check that user exists & password is viable
                if (user != null && VerifyPasswordHash(model.UserPassword, user.UserPassword))
                {
                    // Create claims /w user data + addition to user identity
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserLogin),
                    new Claim(ClaimTypes.Email, user.UserPerson.PersonMail),
                    new Claim(ClaimTypes.StreetAddress, user.UserPerson.PersonAddress),
                    // add more claims
                };

                    // If user is employee
                    if (user.UserIsEmployee.HasValue && user.UserIsEmployee.Value)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                    }

                    // If user is supplier
                    if (user.UserIsSupplier.HasValue && user.UserIsSupplier.Value)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Supplier"));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        // add auth properties
                        ExpiresUtc = DateTime.UtcNow.AddHours(1), // 1hr expire
                        AllowRefresh = true, // session refresh
                        Items = { { "Login", user.UserLogin } }, // var userLogin = User.FindFirst("Login")?.Value; - odwołanie
                        //RedirectUri = "/Home/Index", // redirect

                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // redirect
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nieprawidłowy login lub hasło.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd logowania");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas logowania. Spróbuj ponownie później.");
        }

        return View(model);
    }


    [HttpGet]
    [Authorize] // only authorized users can accress to these method
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            // Wylogowanie użytkownika
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd wylogowywania");
            return RedirectToAction("Login");
        }
    }

    // verify password hashed
    private bool VerifyPasswordHash(string password, string hashedPassword)
    {
        try
        {
            // Porównaj hasło podane przez użytkownika z zahaszowanym hasłem z bazy danych
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd weryfikacji hasła");
            return false;
        }
    }
}
