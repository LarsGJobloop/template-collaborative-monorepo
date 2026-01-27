using Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

// Build connection string from individual environment variables
var requiredVars = new Dictionary<string, string?>
{
    { "POSTGRES_HOST", Environment.GetEnvironmentVariable("POSTGRES_HOST") },
    { "POSTGRES_PORT", Environment.GetEnvironmentVariable("POSTGRES_PORT") },
    { "POSTGRES_DATABASE", Environment.GetEnvironmentVariable("POSTGRES_DATABASE") },
    { "POSTGRES_USER", Environment.GetEnvironmentVariable("POSTGRES_USER") },
    { "POSTGRES_PASSWORD", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") },
};

var missingVars = requiredVars
    .Where(kv => string.IsNullOrWhiteSpace(kv.Value))
    .Select(kv => kv.Key)
    .ToList();

if (missingVars.Count != 0)
{
    throw new Exception($"Missing required environment variables: {string.Join(", ", missingVars)}");
}

var host = requiredVars["POSTGRES_HOST"]!;
var port = int.Parse(requiredVars["POSTGRES_PORT"]!);
var database = requiredVars["POSTGRES_DATABASE"]!;
var user = requiredVars["POSTGRES_USER"]!;
var password = requiredVars["POSTGRES_PASSWORD"]!;

var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

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
