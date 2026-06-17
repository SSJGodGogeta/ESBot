using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ESBot.Domain.Contracts.Session;
using ESBot.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
namespace ESBot.Tests.api;

public class NegativeSessionTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly EsBotDbContext _db;

    public NegativeSessionTest(ApiFactory factory)
    {
        _client = factory.CreateClient();

        _db = factory.Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<EsBotDbContext>();
        DbSeeder.Seed(_db);
    }

    [Fact]
    public async Task GetSessions_Returns404()
    {
        var response = await _client.GetAsync("/v1/Session/nonexistent");
        
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSessionWithEmptyUserId_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/v1/Session", new { userId = "" });
        
        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReadSessionBeforeMessagesHaveBeenSent_Returns200()
    {
        var response = await _client.GetAsync("/v1/Session?id=a667cb37-84e8-4ad7-94c6-7d139636d8a3&IncludeMessages=true");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNull();

        using var doc = System.Text.Json.JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Array);
        root.GetArrayLength().Should().Be(1);

        var first = root[0];
        first.TryGetProperty("messages", out var messages).Should().BeTrue();
        messages.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Array);
        messages.GetArrayLength().Should().Be(0);
    }
}