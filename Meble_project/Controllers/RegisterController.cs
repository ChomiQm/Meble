using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using inzynierka_geska.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

[Route("register")]
public class RegisterController : Controller
{
    private readonly ModelContext _context;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(ModelContext context, ILogger<RegisterController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult User_registered()
    {
        return View();
    }

    [HttpPost("user")]
    public async Task<IActionResult> RegisterUser(User model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Check if the user with the same login already exists
                var existingUser = await _context.Users
                    .SingleOrDefaultAsync(u => u.UserLogin == model.UserLogin);

                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Użytkownik o podanym loginie już istnieje.");
                    return View(model);
                }

                // Hash the password using BCrypt before storing it in the database
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.UserPassword);

                // Create a new User entity with associated Person data
                var newUser = new User
                {
                    UserLogin = model.UserLogin,
                    UserPassword = hashedPassword,
                    UserPerson = new Person
                    {
                        PersonFirstName = model.UserPerson.PersonFirstName,
                        PersonSurname = model.UserPerson.PersonSurname,
                        PersonAddress = model.UserPerson.PersonAddress,
                        PersonMail = model.UserPerson.PersonMail,
                        PersonPhone = model.UserPerson.PersonPhone,
                    }
                };

                // Add the new user to the database
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Logowanie użytkownika po rejestracji
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, newUser.UserLogin),
                    // Dodaj inne claimy, jeśli są potrzebne
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddHours(1),
                    AllowRefresh = true,
                    // Dodaj inne właściwości autentykacji, jeśli są potrzebne
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("User_registered");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd rejestracji");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas rejestracji. Spróbuj ponownie później.");
        }

        return View(model);
    }

    [HttpPost("supplier")]
    public async Task<IActionResult> RegisterSupplier(User model, Supplier supplier)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Check if the user with the same login already exists
                var existingUser = await _context.Users
                    .SingleOrDefaultAsync(u => u.UserLogin == model.UserLogin);

                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Użytkownik o podanym loginie już istnieje.");
                    return View(model);
                }

                // Hash the password using BCrypt before storing it in the database
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.UserPassword);

                // Create a new User entity with associated Person data
                var newUser = new User
                {
                    UserLogin = model.UserLogin,
                    UserPassword = hashedPassword,
                    UserPerson = new Person
                    {
                        PersonFirstName = model.UserPerson.PersonFirstName,
                        PersonSurname = model.UserPerson.PersonSurname,
                        PersonAddress = model.UserPerson.PersonAddress,
                        PersonMail = model.UserPerson.PersonMail,
                        PersonPhone = model.UserPerson.PersonPhone,
                    },
                    // Ustaw inne właściwości użytkownika, jeśli są potrzebne
                    UserIsSupplier = true
                };

                // Add the new user to the database
                _context.Users.Add(newUser);

                // Utwórz dostawcę (supplier) i przypisz go do użytkownika
                var newSupplier = new Supplier
                {
                    // Ustaw właściwości dostawcy, np. nazwę firmy itp.
                    SupplierCompanyName = supplier.SupplierCompanyName,
                    SupplierCompanyAddress = supplier.SupplierCompanyAddress,
                    SupplierCompanyPhone = supplier.SupplierCompanyPhone,
                    SupplierCompanyMail = supplier.SupplierCompanyMail,
                    SupplierPerson = newUser.UserPerson
                };

                _context.Suppliers.Add(newSupplier);

                await _context.SaveChangesAsync();

                // Logowanie użytkownika po rejestracji
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, newUser.UserLogin),
                    // Dodaj inne claimy, jeśli są potrzebne
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddHours(1),
                    AllowRefresh = true,
                    // Dodaj inne właściwości autentykacji, jeśli są potrzebne
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("User_registered");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd rejestracji");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas rejestracji. Spróbuj ponownie później.");
        }

        return View(model);
    }
}
