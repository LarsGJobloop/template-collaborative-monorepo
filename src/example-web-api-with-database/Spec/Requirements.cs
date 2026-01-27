using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Spec;

[Collection("Requirements")]
public class CommentaryRequirementsSpec(WebApplicationFactory<Program> factory) : TestEnvironment(factory)
{
    [Fact]
    public async Task GivenAnActiveServerWithStoredMemories_WhenTheServerIsRestarted_ThenTheMemoriesAreStillRemembered()
    {
        await ExpectFailure.Run("Not yet implemented", async () =>
        {
            // Given an active server with stored memories
            var client = NewClient();
            var commentRequest = CommentFactory.RandomCommentCreateRequest();
            var createResponse = await client.PostAsync("/comments", JsonContent.Create(commentRequest));
            createResponse.EnsureSuccessStatusCode();
            var createdComment = await createResponse.Content.ReadFromJsonAsync<Comment>();
            Assert.NotNull(createdComment);

            // When the server is restarted
            Kill();
            var restartedClient = Start();

            // Then the memories are still remembered
            var memories = await restartedClient.GetFromJsonAsync<Comment[]>("/comments");
            Assert.NotNull(memories);
            Assert.Contains(memories, c =>
                c.Id == createdComment.Id &&
                c.Content == createdComment.Content &&
                c.Alias == createdComment.Alias);
        });
    }
}
