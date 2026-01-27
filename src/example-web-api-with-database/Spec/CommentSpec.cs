using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Net.Http.Json;

namespace Spec;

public record CommentCreateRequest(string Content, string Alias);

public record Comment(Guid Id, string Content, string Alias);

public static class CommentFactory
{
    public static CommentCreateRequest RandomCommentCreateRequest()
    {
        return new CommentCreateRequest("Hello, world!", "John Doe");
    }

    public static CommentCreateRequest[] GetRandomCommentCreateRequests(int count)
    {
        return [.. Enumerable.Range(0, count).Select(_ => RandomCommentCreateRequest())];
    }

    public static async Task SeedComments(HttpClient client, params CommentCreateRequest[] commentRequests)
    {
        foreach (var commentRequest in commentRequests)
        {
            var response = await client.PostAsync("/comments", JsonContent.Create(commentRequest));
            response.EnsureSuccessStatusCode();
        }
    }
}

[Collection("CRUD Operations")]
public class CommentaryRequestSpec(WebApplicationFactory<Program> factory) : TestEnvironment(factory)
{
    [Fact]
    public async Task GivenAValidComment_WhenTheCommentIsRecived_ThenTheCommentIsReturned()
    {
        // Given a valid comment
        var client = NewClient();
        var validCommentCreateRequest = CommentFactory.RandomCommentCreateRequest();

        // When the comment is received
        var response = await client.PostAsync("/comments", JsonContent.Create(validCommentCreateRequest));
        response.EnsureSuccessStatusCode();

        // Then the comment is returned
        var comment = await response.Content.ReadFromJsonAsync<Comment>();
        Assert.NotNull(comment);
        Assert.Equal(validCommentCreateRequest.Content, comment.Content);
        Assert.Equal(validCommentCreateRequest.Alias, comment.Alias);
    }

    [Fact]
    public async Task GivenAnInvalidComment_WhenTheCommentIsSent_AnErrorWithTheDocumentationLinkIsReturned()
    {
        // Given an invalid comment
        var client = NewClient();
        var invalidCommentCreateRequest = new CommentCreateRequest("", "");

        // When the comment is received
        var response = await client.PostAsync("/comments", JsonContent.Create(invalidCommentCreateRequest));
        Assert.False(response.IsSuccessStatusCode);
        // And the response indicate a client error
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }
}

[Collection("Querys Operations")]
public class CommentaryListSpec(WebApplicationFactory<Program> factory) : TestEnvironment(factory)
{
    [Theory]
    [InlineData(5, 5)]   // Few comments - all returned
    [InlineData(10, 10)] // Exactly enough - all returned
    [InlineData(15, 10)] // Many comments - limit to 10
    public async Task GivenExistingComments_WhenTheLatestCommentsAreRequested_ThenTheLatestCommentsAreReturnedInLIFOOrder(
        int commentCount, int expectedReturned)
    {
        // Given existing comments
        var client = NewClient();
        var commentRequests = CommentFactory.GetRandomCommentCreateRequests(commentCount);
        await CommentFactory.SeedComments(client, commentRequests);

        // When the latest comments are requested
        var response = await client.GetAsync("/comments");
        response.EnsureSuccessStatusCode();

        // Then the latest comments are returned in LIFO order (newest first)
        var comments = await response.Content.ReadFromJsonAsync<Comment[]>();
        Assert.NotNull(comments);
        Assert.Equal(expectedReturned, comments.Length);
        
        // Verify LIFO ordering: newest comments first (reverse of creation order)
        var expectedComments = commentRequests
            .TakeLast(expectedReturned)
            .Reverse()
            .ToArray();
            
        for (int i = 0; i < expectedComments.Length; i++)
        {
            Assert.Equal(expectedComments[i].Content, comments[i].Content);
            Assert.Equal(expectedComments[i].Alias, comments[i].Alias);
        }
    }
}
