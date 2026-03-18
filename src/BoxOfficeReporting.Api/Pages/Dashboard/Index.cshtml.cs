using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoxOfficeReporting.Api.Pages;

public class DashboardModel : PageModel
{
    public string CurrentTime { get; private set; } = string.Empty;

    public void OnGet()
    {
        CurrentTime = DateTime.Now.ToString("f");
    }
}
