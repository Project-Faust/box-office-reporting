using System.ComponentModel.DataAnnotations;
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
    public EditReportInputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var report = await _context
            .ReportEntries.Include(r => r.Events)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (report == null)
        {
            return NotFound();
        }

        Input.Id = report.Id;
        Input.ReportDate = report.ReportDate;
        Input.TicketsSold = report.TicketsSold;
        Input.PricePerTicket = report.PricePerTicket;

        var events = report.Events.ToList();

        if (events.Count > 0)
        {
            Input.EventName1 = events[0].EventName;
            Input.DeductionPercent1 = events[0].DeductionPercent;
        }

        if (events.Count > 1)
        {
            Input.EventName2 = events[1].EventName;
            Input.DeductionPercent2 = events[1].DeductionPercent;
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

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingReport = await _context
            .ReportEntries.Include(r => r.Events)
            .FirstOrDefaultAsync(r => r.Id == Input.Id && r.UserId == userId);

        if (existingReport == null)
        {
            return NotFound();
        }

        existingReport.ReportDate = Input.ReportDate;
        existingReport.TicketsSold = Input.TicketsSold;
        existingReport.PricePerTicket = Input.PricePerTicket;

        _context.ReportEvents.RemoveRange(existingReport.Events);
        existingReport.Events.Clear();

        AddEventIfProvided(existingReport, Input.EventName1, Input.DeductionPercent1);
        AddEventIfProvided(existingReport, Input.EventName2, Input.DeductionPercent2);

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

    public class EditReportInputModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Tickets sold cannot be negative.")]
        public int TicketsSold { get; set; }

        [Required]
        [Range(
            typeof(decimal),
            "0",
            "79228162514264337593543950335",
            ErrorMessage = "Price per ticket cannot be negative."
        )]
        public decimal PricePerTicket { get; set; }

        public string? EventName1 { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "79228162514264337593543950335",
            ErrorMessage = "Deduction percent cannot be negative."
        )]
        public decimal? DeductionPercent1 { get; set; }

        public string? EventName2 { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "79228162514264337593543950335",
            ErrorMessage = "Deduction percent cannot be negative."
        )]
        public decimal? DeductionPercent2 { get; set; }
    }
}
