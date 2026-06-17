using FluentAssertions;
using RestSharp;

namespace ESBot.Tests.api;

public class NegativeSessionTest
{

    [Fact]
    public async Task GetSessions_Returns404()
    {
        RestClient client = new RestClient("http://localhost:5243");
        RestRequest request = new RestRequest("/v1/Session/nonexistent", Method.Get);
        
        RestResponse response = await client.ExecuteAsync(request);
        
        response.StatusCode.Should()
            .Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSessionWithEmptyUserId_Returns400()
    {
        RestClient client = new RestClient("http://localhost:5243");
        RestRequest request = new RestRequest("/v1/Session", Method.Post);
        request.AddJsonBody(new { userId = "" });

        RestResponse response = await client.ExecuteAsync(request);
        
        response.StatusCode.Should()
            .Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReadSessionBeforeMessagesHaveBeenSent_Returns200()
    {
        RestClient client = new RestClient("http://localhost:5243");
        RestRequest request = new RestRequest("/v1/Session?id=a667cb37-84e8-4ad7-94c6-7d139636d8a3&IncludeMessages=true", Method.Get);

        RestResponse response = await client.ExecuteAsync(request);

        response.StatusCode.Should()
            .Be(System.Net.HttpStatusCode.OK);

        response.Content.Should().NotBeNull();

        using var doc = System.Text.Json.JsonDocument.Parse(response.Content!);
        var root = doc.RootElement;

        root.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Array);
        root.GetArrayLength().Should().Be(1);

        var first = root[0];
        first.TryGetProperty("messages", out var messages).Should().BeTrue();
        messages.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Array);
        messages.GetArrayLength().Should().Be(0);
    }
}