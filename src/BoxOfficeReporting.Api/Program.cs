using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// protect everything by default
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet(
        "/api",
        () =>
        {
            if (app.Environment.IsDevelopment())
            {
                return Results.Ok(
                    new
                    {
                        service = "BoxOfficeReporting.Api",
                        status = "running",
                        endpoints = "/health",
                    }
                );
            }

            return Results.Ok(new { service = "BoxOfficeReporting.Api", status = "running" });
        }
    )
    .WithName("ApiRoot");

app.MapGet(
        "/health",
        () =>
            Results.Ok(
                new
                {
                    service = "BoxOfficeReporting.Api",
                    status = "ok",
                    environment = app.Environment.EnvironmentName,
                }
            )
    )
    .AllowAnonymous()
    .WithName("HealthCheck");

app.Run();
