using System.Security.Claims;
using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoxOfficeReporting.Api.Pages.Reports;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string Range { get; set; } = "30days";

    [BindProperty(SupportsGet = true)]
    public int? Year { get; set; }

    public IList<ReportEntry> ReportEntries { get; set; } = new List<ReportEntry>();

    public IList<ReportGroupViewModel> ReportGroups { get; set; } =
        new List<ReportGroupViewModel>();

    public ReportsSummaryViewModel Summary { get; set; } = new();

    public IList<int> AvailableYears { get; set; } = new List<int>();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            ReportEntries = new List<ReportEntry>();
            ReportGroups = new List<ReportGroupViewModel>();
            Summary = new ReportsSummaryViewModel();
            AvailableYears = new List<int>();
            return;
        }

        AvailableYears = await _context
            .ReportEntries.Where(r => r.UserId == userId)
            .Select(r => r.ReportDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        if (Range == "year")
        {
            if (!Year.HasValue || !AvailableYears.Contains(Year.Value))
            {
                Year = AvailableYears.Any() ? AvailableYears.First() : null;
            }
        }

        var query = _context.ReportEntries.Include(r => r.Events).Where(r => r.UserId == userId);

        var today = DateTime.Today;

        query = Range switch
        {
            "7days" => query.Where(r => r.ReportDate >= today.AddDays(-7)),
            "30days" => query.Where(r => r.ReportDate >= today.AddDays(-30)),
            "year" when Year.HasValue => query.Where(r => r.ReportDate.Year == Year.Value),
            "all" => query,
            _ => query.Where(r => r.ReportDate >= today.AddDays(-30)),
        };

        ReportEntries = await query
            .OrderByDescending(r => r.ReportDate)
            .ThenByDescending(r => r.Id)
            .ToListAsync();

        Summary = new ReportsSummaryViewModel
        {
            TotalReports = ReportEntries.Count,
            TotalTicketsSold = ReportEntries.Sum(r => r.TicketsSold),
            TotalGross = ReportEntries.Sum(r => r.Gross),
            TotalDeductions = ReportEntries.Sum(r => r.TotalDeductionAmount),
            TotalNet = ReportEntries.Sum(r => r.Net),
        };

        ReportGroups = ReportEntries
            .GroupBy(r => r.ReportDate.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new ReportGroupViewModel
            {
                ReportDate = g.Key,
                Entries = g.OrderByDescending(r => r.Id).ToList(),
                TotalReports = g.Count(),
                TotalTicketsSold = g.Sum(r => r.TicketsSold),
            })
            .ToList();
    }

    public class ReportsSummaryViewModel
    {
        public int TotalReports { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }
    }

    public class ReportGroupViewModel
    {
        public DateTime ReportDate { get; set; }
        public IList<ReportEntry> Entries { get; set; } = new List<ReportEntry>();
        public int TotalReports { get; set; }
        public int TotalTicketsSold { get; set; }
    }
}
