var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// protect everything by default
// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization(options =>
// {
    // options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
// });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/health", () => Results.Ok("ok")).AllowAnonymous();

app.Run();
