using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet(
        "/",
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
    .WithName("Root");

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
    .WithName("HealthCheck");

app.Run();
