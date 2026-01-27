using System.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddSingleton<CommentStore>();

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.MapPost("/comments", (CommentCreateRequest request, CommentStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Content) || string.IsNullOrWhiteSpace(request.Alias))
    {
        return Results.BadRequest();
    }

    var comment = new Comment(
        Id: Guid.NewGuid(),
        Content: request.Content,
        Alias: request.Alias
    );
    store.Add(comment);
    return Results.Ok(comment);
});

app.MapGet("/comments", (CommentStore store) => store.GetAll());

app.Run();

public record CommentCreateRequest(string Content, string Alias);
public record Comment(Guid Id, string Content, string Alias);

public class CommentStore
{
    private readonly List<Comment> _comments = new();
    
    public void Add(Comment comment) => _comments.Add(comment);
    
    public Comment[] GetAll() => _comments
        .TakeLast(10)
        .Reverse()
        .ToArray();
    
    public void Clear() => _comments.Clear();
}
