using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RaorPages;

namespace BoxOfficeReporting.Api.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<Applicationuser> _signInManager;

    public LogoutModel(SignInManager<Applicationuser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPost()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Index");
    }
}
