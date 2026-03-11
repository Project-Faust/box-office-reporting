using BoxOfficeReporting.Api.Data;
using BoxOfficeReporting.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
    options.SignIn.RequireConfirmedAccount = false;
    })
.AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

  builder.Services.AddRazorPages();

  var app = builder.Build();

  app.UseHttpsRedirection();
  app.UseStaticFiles();

  app.UseRouting();

  app.UseAuthentication();
  app.UseAuthorization();

  app.MapRazorPages();

  app.MapGet("/health", () => Results.Ok("ok")).AllowAnonymous();

  app.Run();
