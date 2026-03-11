using System.ComponentModel.DataAnnotations;

namespace BoxOfficeReporting.Api.Models;

public class ReportEvent
{
  public int Id { get; set; }

  [Required]
  public int ReportEntryId { get; set; }

  public ReportEntry ReportEntry { get; set; } = default!;

  [Required]
  [StringLength(200)]
  public string EventName { get; set; } = string.Empty;

  [Range(0, 100)]
  public decimal DeductionPercent { get; set; }
}
