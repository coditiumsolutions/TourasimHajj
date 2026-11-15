using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Models;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Controllers;

[AllowAnonymous]
public class CustomerAccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _db;

    public CustomerAccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new CustomerLoginViewModel { ReturnUrl = returnUrl });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(CustomerLoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await ResolveUserAsync(model.EmailOrMobile);
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (!await _userManager.IsInRoleAsync(user, AppRoles.Customer))
        {
            ModelState.AddModelError(string.Empty, "This account is not a customer account. Use Admin Login.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Dashboard", "CustomerPortal");
    }

    [HttpGet]
    public IActionResult Register() => View(new CustomerRegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CustomerRegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var emailExists = await _userManager.FindByEmailAsync(model.Email);
        if (emailExists != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already registered.");
            return View(model);
        }

        var mobileExists = await _db.Customers.AnyAsync(c => c.MobileNo == model.MobileNo);
        if (mobileExists)
        {
            ModelState.AddModelError(nameof(model.MobileNo), "Mobile number is already registered.");
            return View(model);
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var user = new ApplicationUser
            {
                UserName = model.Email.Trim(),
                Email = model.Email.Trim(),
                EmailConfirmed = true,
                PhoneNumber = model.MobileNo,
                FullName = model.FullName.Trim(),
                Designation = "Customer",
                IsActive = true
            };

            var create = await _userManager.CreateAsync(user, model.Password);
            if (!create.Succeeded)
            {
                foreach (var error in create.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, AppRoles.Customer);

            _db.Customers.Add(new Customer
            {
                CustomerName = model.FullName.Trim(),
                FatherName = model.FatherName,
                MobileNo = model.MobileNo.Trim(),
                WhatsAppNo = model.WhatsAppNo,
                Email = model.Email.Trim(),
                Cnic = model.Cnic,
                CustomerType = "Individual",
                AspNetUserId = user.Id,
                CreatedDate = DateTime.Now,
                IsActive = true
            });
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = "Welcome! Your account has been created.";
            return RedirectToAction("Dashboard", "CustomerPortal");
        }
        catch
        {
            await tx.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            return View(model);
        }
    }

    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private async Task<ApplicationUser?> ResolveUserAsync(string emailOrMobile)
    {
        var key = emailOrMobile.Trim();
        var byEmail = await _userManager.FindByEmailAsync(key);
        if (byEmail != null) return byEmail;

        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.MobileNo == key && c.AspNetUserId != null);
        if (customer?.AspNetUserId == null) return null;
        return await _userManager.FindByIdAsync(customer.AspNetUserId);
    }
}
