using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoxOfficeReporting.Api.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public void OnGet() { }

    public IActionResult OnPostGoogle(string? returnUrl = null)
    {
        var redirectUrl = Url.Page(
            "/Account/ExternalLogin",
            pageHandler: "Callback",
            values: new { returnUrl }
        );

        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme,
            redirectUrl
        );

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }
}
