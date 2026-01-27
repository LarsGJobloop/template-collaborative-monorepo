using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace Spec;

[Collection("Requirements")]
public class CommentaryRequirementsSpec(WebApplicationFactory<Program> factory) : TestEnvironment(factory)
{
    [Fact]
    public async Task GivenAnActiveServerWithStoredMemories_WhenTheServerIsRestarted_ThenTheMemoriesAreStillRemembered()
    {
        await ExpectFailure.Run("Not yet implemented", async () =>
        {
            // Given an active server
            var client = NewClient();

            // And stored memories
            var commentRequest = CommentFactory.RandomCommentCreateRequest();
            var comments = await CommentFactory.SeedComments(client, commentRequest);
            Assert.Single(comments);

            // When the server is restarted
            var restartedClient = ResetApp();

            // Then the stored memories are still remembered
            var restartedComments = await restartedClient.GetFromJsonAsync<Comment[]>("/comments");
            Assert.NotNull(restartedComments);
            Assert.Single(restartedComments);
            Assert.Equal(comments[0].Id, restartedComments[0].Id);
            Assert.Equal(comments[0].Content, restartedComments[0].Content);
            Assert.Equal(comments[0].Alias, restartedComments[0].Alias);
        });
    }
}
