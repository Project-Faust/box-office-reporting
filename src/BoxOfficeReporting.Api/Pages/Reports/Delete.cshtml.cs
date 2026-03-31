using System.Security.Claims;
using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoxOfficeReporting.Api.Pages.Reports;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public ReportEntry ReportEntry { get; set; } = new();

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

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var report = await _context
            .ReportEntries.Include(r => r.Events)
            .FirstOrDefaultAsync(r => r.Id == ReportEntry.Id && r.UserId == userId);

        if (report == null)
        {
            return NotFound();
        }

        _context.ReportEvents.RemoveRange(report.Events);
        _context.ReportEntries.Remove(report);

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
