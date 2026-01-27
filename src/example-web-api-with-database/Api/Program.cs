using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.MapPost("/comments", (CommentCreateRequest request) =>
{
    var comment = new Comment(
        Id: Guid.NewGuid(),
        Content: request.Content,
        Alias: request.Alias
    );
    return Results.Ok(comment);
});

app.Run();

public record CommentCreateRequest(string Content, string Alias);
public record Comment(Guid Id, string Content, string Alias);
