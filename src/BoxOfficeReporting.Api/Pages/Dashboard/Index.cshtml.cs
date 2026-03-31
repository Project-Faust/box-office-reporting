using System.Security.Claims;
using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BoxOfficeReporting.Api.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public DashboardSummaryViewModel WeekSummary { get; set; } = new();
    public DashboardSummaryViewModel MonthSummary { get; set; } = new();
    public DashboardSummaryViewModel YearSummary { get; set; } = new();

    public IList<DailyTrendPointViewModel> Last30Days { get; set; } =
        new List<DailyTrendPointViewModel>();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var today = DateTime.Today;
        var weekStart = today.AddDays(-7);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var yearStart = new DateTime(today.Year, 1, 1);

        var reports = await _context
            .ReportEntries.Include(r => r.Events)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.ReportDate)
            .ToListAsync();

        WeekSummary = BuildSummary(reports.Where(r => r.ReportDate >= weekStart));
        MonthSummary = BuildSummary(reports.Where(r => r.ReportDate >= monthStart));
        YearSummary = BuildSummary(reports.Where(r => r.ReportDate >= yearStart));

        Last30Days = reports
            .Where(r => r.ReportDate >= today.AddDays(-30))
            .OrderBy(r => r.ReportDate)
            .Select(r => new DailyTrendPointViewModel
            {
                Label = r.ReportDate.ToString("MM/dd"),
                TicketsSold = r.TicketsSold,
                Gross = r.Gross,
                TotalDeductions = r.TotalDeductionAmount,
                Net = r.Net,
            })
            .ToList();
    }

    private static DashboardSummaryViewModel BuildSummary(IEnumerable<ReportEntry> reports)
    {
        var reportList = reports.ToList();

        return new DashboardSummaryViewModel
        {
            TotalReports = reportList.Count,
            TotalTicketsSold = reportList.Sum(r => r.TicketsSold),
            TotalGross = reportList.Sum(r => r.Gross),
            TotalDeductions = reportList.Sum(r => r.TotalDeductionAmount),
            TotalNet = reportList.Sum(r => r.Net),
        };
    }

    public class DashboardSummaryViewModel
    {
        public int TotalReports { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }
    }

    public class DailyTrendPointViewModel
    {
        public string Label { get; set; } = string.Empty;
        public int TicketsSold { get; set; }
        public decimal Gross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal Net { get; set; }
    }
}
