using System.ComponentModel.DataAnnotations;
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
    public CreateReportInputModel Input { get; set; } = new();

    public void OnGet()
    {
        Input.ReportDate = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var reportEntry = new ReportEntry
        {
            UserId = userId,
            ReportDate = Input.ReportDate,
            TicketsSold = Input.TicketsSold,
            PricePerTicket = Input.PricePerTicket,
        };

        AddEventIfProvided(reportEntry, Input.EventName1, Input.DeductionPercent1);
        AddEventIfProvided(reportEntry, Input.EventName2, Input.DeductionPercent2);

        _context.ReportEntries.Add(reportEntry);
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

    public class CreateReportInputModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [Required]
        public int TicketsSold { get; set; }

        [Required]
        public decimal PricePerTicket { get; set; }

        public string? EventName1 { get; set; }
        public decimal? DeductionPercent1 { get; set; }

        public string? EventName2 { get; set; }
        public decimal? DeductionPercent2 { get; set; }
    }
}
