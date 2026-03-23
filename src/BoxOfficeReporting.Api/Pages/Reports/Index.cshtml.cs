using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BoxOfficeReporting.Api.Pages.Reports;

[Authorize]
public class IndexModel : PageModel {
  private readonly ApplicationDbContext _context;

  public IndexModel(ApplicationDbContext context) {
    _context = context;
  }

  public IList<ReportEntry> ReportEntries { get; set; } = new List<ReportEntry>();

  public async Task OnGetAsync() {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    ReportEntries = await _context.ReportEntries
      .Include(r => r.Events)
      .Where(r => r.UserId == userId)
      .OrderByDescending(r => r.ReportDate)
      .ToListAsync();
  }
}
