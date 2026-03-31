using System.Security.Claims;
using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoxOfficeReporting.Api.Pages.Reports;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
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

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var report = await _context
            .ReportEntries.Include(r => r.Events)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (report == null)
        {
            return NotFound();
        }

        ReportEntry = report;

        var events = report.Events.ToList();

        if (events.Count > 0)
        {
            EventName1 = events[0].EventName;
            DeductionPercent1 = events[0].DeductionPercent;
        }

        if (events.Count > 1)
        {
            EventName2 = events[1].EventName;
            DeductionPercent2 = events[1].DeductionPercent;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        ModelState.Remove("ReportEntry.UserId");
        ModelState.Remove("ReportEntry.User");
        ModelState.Remove("ReportEntry.Events");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingReport = await _context
            .ReportEntries.Include(r => r.Events)
            .FirstOrDefaultAsync(r => r.Id == ReportEntry.Id && r.UserId == userId);

        if (existingReport == null)
        {
            return NotFound();
        }

        existingReport.ReportDate = ReportEntry.ReportDate;
        existingReport.TicketsSold = ReportEntry.TicketsSold;
        existingReport.PricePerTicket = ReportEntry.PricePerTicket;

        _context.ReportEvents.RemoveRange(existingReport.Events);

        AddEventIfProvided(existingReport, EventName1, DeductionPercent1);
        AddEventIfProvided(existingReport, EventName2, DeductionPercent2);

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private static void AddEventIfProvided(
        ReportEntry reportEntry,
        string? eventName,
        decimal? deductionPercent
    )
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return;
        }

        reportEntry.Events.Add(
            new ReportEvent
            {
                EventName = eventName.Trim(),
                DeductionPercent = deductionPercent ?? 0,
            }
        );
    }
}
