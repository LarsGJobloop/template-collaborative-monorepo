using Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var connectionString = builder.Configuration["ConnectionStrings:Commentary"]
    ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("POSTGRES_CONNECTION_STRING is not set");
}
builder.Services.AddDbContextPool<CommentaryContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<CommentaryService>();

var app = builder.Build();

// Apply migrations on every startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CommentaryContext>();
    context.Database.Migrate();
}

app.MapHealthChecks("/healthz");

app.MapPost("/comments", async (CommentCreateRequest request, CommentaryService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Content) || string.IsNullOrWhiteSpace(request.Alias))
    {
        return Results.BadRequest();
    }

    var comment = new Comment(request);
    await service.Add(comment);
    return Results.Ok(comment);
});

app.MapGet("/comments", (CommentaryService service) => Results.Ok(service.GetAll()));

app.Run();
