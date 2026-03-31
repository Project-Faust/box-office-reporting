using System.Security.Claims;
using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoxOfficeReporting.Api.Pages.Reports;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public ReportEntry ReportEntry { get; set; } = new();

    [BindProperty]
    public string? EventName1 { get; set; }

    [BindProperty]
    public decimal? DeductionPercent1 { get; set; }

    [BindProperty]
    public string? EventName2 { get; set; }

    [BindProperty]
    public decimal? DeductionPercent2 { get; set; }

    [BindProperty]
    public string? EventName3 { get; set; }

    [BindProperty]
    public decimal? DeductionPercent3 { get; set; }

    public void OnGet()
    {
        ReportEntry.ReportDate = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        ReportEntry.UserId = userId;

        ModelState.Remove("ReportEntry.UserId");
        ModelState.Remove("ReportEntry.User");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        AddEventIfProvided(EventName1, DeductionPercent1);
        AddEventIfProvided(EventName2, DeductionPercent2);

        _context.ReportEntries.Add(ReportEntry);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private void AddEventIfProvided(string? eventName, decimal? deductionPercent)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return;
        }

        ReportEntry.Events.Add(
            new ReportEvent
            {
                EventName = eventName.Trim(),
                DeductionPercent = deductionPercent ?? 0,
            }
        );
    }
}
