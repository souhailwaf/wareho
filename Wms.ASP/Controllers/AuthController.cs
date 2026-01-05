using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Wms.Domain.Services;

namespace Wms.ASP.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // If already logged in, redirect to dashboard
        if (IsAuthenticated())
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string usernameOrEmail, string password, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Username/Email and password are required.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var user = await _authService.AuthenticateAsync(usernameOrEmail, password);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid username/email or password.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Store user info in session
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("FullName", user.FullName);
        HttpContext.Session.SetString("Email", user.Email);

        _logger.LogInformation("User {Username} logged in successfully", user.Username);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var username = HttpContext.Session.GetString("Username");
        
        HttpContext.Session.Clear();
        
        _logger.LogInformation("User {Username} logged out", username ?? "Unknown");
        
        return RedirectToAction("Login");
    }

    private bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
    }
}

