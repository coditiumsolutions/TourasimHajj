using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourismHajj.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    public IActionResult OnGet(string? returnUrl = null)
    {
        var url = "/Account/Login";
        if (!string.IsNullOrWhiteSpace(returnUrl))
            url += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
        return Redirect(url);
    }

    public IActionResult OnPost(string? returnUrl = null) => OnGet(returnUrl);
}
