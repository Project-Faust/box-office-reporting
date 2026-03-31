using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BoxOfficeReporting.Api.Models;

public class ReportEntry
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = default!;

    public ApplicationUser User { get; set; } = default!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReportDate { get; set; }

    [Range(0, int.MaxValue)]
    public int TicketsSold { get; set; }

    [Range(0, 999999.99)]
    public decimal PricePerTicket { get; set; }

    public ICollection<ReportEvent> Events { get; set; } = new List<ReportEvent>();

    public decimal Gross => TicketsSold * PricePerTicket;

    public decimal TotalDeductionAmount => Events.Sum(e => Gross * (e.DeductionPercent / 100m));

    public decimal Net => Gross - TotalDeductionAmount;
}
