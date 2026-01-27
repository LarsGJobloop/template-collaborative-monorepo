using Microsoft.EntityFrameworkCore;

namespace Api;

public class CommentaryContext(DbContextOptions<CommentaryContext> options) : DbContext(options)
{
  public DbSet<Comment> Comments { get; set; } = null!;
}

public record CommentCreateRequest(string Content, string Alias);
public class Comment
{
  public Guid Id { get; set; }
  public string Content { get; set; }
  public string Alias { get; set; }
  public DateTime CreatedAt { get; set; }

  public Comment(CommentCreateRequest request)
  {
    Id = Guid.NewGuid();
    Content = request.Content;
    Alias = request.Alias;
    CreatedAt = DateTime.UtcNow;
  }

  public Comment(Guid id, string content, string alias, DateTime createdAt)
  {
    Id = id;
    Content = content;
    Alias = alias;
    CreatedAt = createdAt;
  }
}

public class CommentaryService(CommentaryContext context)
{
  private readonly CommentaryContext _context = context;

  public async Task<int> Add(Comment comment)
  {
    _context.Comments.Add(comment);
    return await _context.SaveChangesAsync();
  }

  public Comment[] GetAll() => [.. _context.Comments
    .OrderByDescending(comment => comment.CreatedAt)
    .Take(10)];
}
