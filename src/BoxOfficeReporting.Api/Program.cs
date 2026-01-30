using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet(
        "/health",
        () =>
            Results.Ok(
                new
                {
                    status = "ok",
                    service = "BoxOfficeReporting.Api",
                    environment = app.Environment.EnvironmentName,
                }
            )
    )
    .WithName("HealthCheck");

app.Run();
