using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
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
        await ExpectFailure.Run("Not yet implemented", async () =>
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
        });
    }

    [Fact]
    public async Task GivenAnInvalidComment_WhenTheCommentIsSent_AnErrorWithTheDocumentationLinkIsReturned()
    {
        await ExpectFailure.Run("Not yet implemented", async () =>
        {
            // Given an invalid comment
            var client = NewClient();
            var invalidCommentCreateRequest = new CommentCreateRequest("", "");

            // When the comment is received
            var response = await client.PostAsync("/comments", JsonContent.Create(invalidCommentCreateRequest));
            Assert.False(response.IsSuccessStatusCode);
            // And the response indicate a client error
            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        });
    }
}

[Collection("Querys Operations")]
public class CommentaryListSpec(WebApplicationFactory<Program> factory) : TestEnvironment(factory)
{
    [Fact]
    public async Task GivenExistingComments_WhenTheLatestCommentsAreRequested_ThenTheLatestCommentsAreReturned()
    {
        await ExpectFailure.Run("Not yet implemented", async () =>
        {
            // Given existing comments
            var client = NewClient();
            var commentRequests = CommentFactory.GetRandomCommentCreateRequests(10);
            await CommentFactory.SeedComments(client, commentRequests);

            // When the latest comments are requested
            var response = await client.GetAsync("/comments");
            response.EnsureSuccessStatusCode();

            // Then the latest comments are returned
            var comments = await response.Content.ReadFromJsonAsync<Comment[]>();
            Assert.NotNull(comments);
            Assert.Equal(commentRequests.Length, comments.Length);
            for (int i = 0; i < comments.Length; i++)
            {
                Assert.Equal(commentRequests[i].Content, comments[i].Content);
                Assert.Equal(commentRequests[i].Alias, comments[i].Alias);
            }
        });
    }

    [Fact]
    public async Task GivenAFewExistingComments_WhenClientsRequestTheLatestComments_ThenAllCommentsAreReturned()
    {
        await ExpectFailure.Run("Not yet implemented", async () =>
        {
            // Given existing comments
            var client = NewClient();
            var commentRequests = CommentFactory.GetRandomCommentCreateRequests(5);
            await CommentFactory.SeedComments(client, commentRequests);

            // When clients request the latest comments
            var response = await client.GetAsync("/comments");
            response.EnsureSuccessStatusCode();

            // Then all comments are returned
            var comments = await response.Content.ReadFromJsonAsync<Comment[]>();
            Assert.NotNull(comments);
            Assert.Equal(commentRequests.Length, comments.Length);
        });
    }

    [Fact]
    public async Task GivenManyExistingComments_WhenClientsRequestTheLatestComments_ThenNoMoreThanTenAreReturned()
    {
        await ExpectFailure.Run("Not yet implemented", async () =>
        {
            // Given existing comments
            var client = NewClient();
            var commentRequests = CommentFactory.GetRandomCommentCreateRequests(15);
            await CommentFactory.SeedComments(client, commentRequests);

            // When clients request the latest comments
            var response = await client.GetAsync("/comments");
            response.EnsureSuccessStatusCode();

            // Then no more than 10 comments are returned
            var comments = await response.Content.ReadFromJsonAsync<Comment[]>();
            Assert.NotNull(comments);
            Assert.True(comments.Length <= 10, "No more than 10 comments should be returned");
        });
    }
}
