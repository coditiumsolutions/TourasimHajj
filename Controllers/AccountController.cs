using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourismHajj.Helpers;
using TourismHajj.Models;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        // Already signed in as staff → open management site immediately.
        if (User.Identity?.IsAuthenticated == true && AppRoles.Staff.Any(User.IsInRole))
            return RedirectToAction("Index", "Dashboard");

        // If a customer session is active, clear it so admin can sign in cleanly.
        if (User.Identity?.IsAuthenticated == true)
            await _signInManager.SignOutAsync();

        return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Invalid admin login attempt.");
            return View(model);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any(r => AppRoles.Staff.Contains(r)))
        {
            ModelState.AddModelError(string.Empty, "This account is not authorized for Admin access. Use Customer Login.");
            return View(model);
        }

        // Clear any existing customer cookie before opening the ERP.
        if (User.Identity?.IsAuthenticated == true)
            await _signInManager.SignOutAsync();

        // Sign in by userName (email) so Identity lookup stays consistent with seeded accounts.
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName ?? model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid admin login attempt.");
            return View(model);
        }

        // Prefer management dashboard; only honor returnUrl for admin routes.
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
            && Url.IsLocalUrl(model.ReturnUrl)
            && !model.ReturnUrl.StartsWith("/Customer", StringComparison.OrdinalIgnoreCase)
            && model.ReturnUrl != "/")
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
