using System.Net;
using System.Net.Http.Json;
using ESBot.Domain.Contracts.Session;
using ESBot.Domain.Contracts.User;
using ESBot.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ESBot.Tests.api;

public class SessionTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly EsBotDbContext _db;

    public SessionTest(ApiFactory factory)
    {
        _client = factory.CreateClient();

        _db = factory.Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<EsBotDbContext>();
        DbSeeder.Seed(_db);
    }

    [Fact]
    public async Task GetSessions_Returns200()
    {
        var response = await _client.GetAsync("/v1/Session");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateSession_Returns201_With_SessionId()
    {
        var user = _db.Users.First();

        var dto = new CreateSessionDto
        {
            UserId = user.Id,
            Title = "Test Session"
        };

        var response = await _client.PostAsJsonAsync(
            "/v1/Session",
            dto);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var session = await response.Content
            .ReadFromJsonAsync<SessionDto>();

        session.Should().NotBeNull();

        session!.Id.Should().NotBe(Guid.Empty);
        session.UserId.Should().Be(user.Id);
        session.Title.Should().Be("Test Session");

        session.StartedAt.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        session.EndedAt.Should().BeNull();

        session.MessageCount.Should().Be(0);
        session.QuizRequestCount.Should().Be(0);
    }

    [Fact]
    public async Task GetSessionsForCreatedUser_Returns200_And_Sessions()
    {
        // 1. Benutzer erstellen
        var createUser = new CreateUserDto
        {
            Email = "arman@arman.com",
            Password = "random",
            Username = "arman"
        };
        var userResponse = await _client.PostAsJsonAsync("/v1/User", createUser);
        userResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();

        // 2. Session für diesen Benutzer erstellen
        var createSessionDto = new CreateSessionDto
        {
            Title = "Arman's Session",
            UserId = user.Id // direkt aus dem DTO
        };
        var sessionResponse = await _client.PostAsJsonAsync("/v1/Session", createSessionDto);
        sessionResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var session = await sessionResponse.Content.ReadFromJsonAsync<SessionDto>();
        session.Should().NotBeNull();

        // Detailprüfungen der Session
        session.Id.Should().NotBe(Guid.Empty);
        session.UserId.Should().Be(user.Id);
        session.Title.Should().Be("Arman's Session");
        session.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        session.EndedAt.Should().BeNull();
        session.MessageCount.Should().Be(0);
        session.QuizRequestCount.Should().Be(0);

        // 3. Alle Sessions abrufen
        var getSessionsResponse = await _client.GetAsync("/v1/Session");
        getSessionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var sessions = await getSessionsResponse.Content.ReadFromJsonAsync<List<SessionDto>>();
        sessions.Should().NotBeNull();
        sessions.Should().NotBeEmpty();

        // 4. Prüfen, ob die erstellte Session in der Liste ist
        sessions.Should().ContainSingle(s => s.Id == session.Id);
    }
}