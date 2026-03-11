using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoxOfficeReporting.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  public DbSet<ReportEntry> ReportEntries => Set<ReportEntry>();
  public DbSet<ReportEvent> ReportEvents => Set<ReportEvent>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<ReportEntry>()
      .HasMany(r => r.Events)
      .WithOne(e => e.ReportEntry)
      .HasForeignKey(e => e.ReportEntryId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
