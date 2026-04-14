using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class UserSessionEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public UserSessionEntityTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<EsBotDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new EsBotDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void Session_Creation_WithValidData_ShouldSucceed()
    {
        var user = new User { Username = "suser", Email = "s@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.SaveChanges();

        var saved = _context.UserSessions.Include(s => s.User).FirstOrDefault(s => s.Id == session.Id);
        Assert.NotNull(saved);
        Assert.Equal(user.Id, saved.UserId);
        Assert.Equal(user.Username, saved.User.Username);
    }

    [Fact]
    public void Session_Helper_AddMessage_ShouldLinkBothSides()
    {
        var user = new User { Username = "smsguser", Email = "su@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var message = new Message { Content = "hi", Role = EMessageRole.User };

        session.AddMessage(message);

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.Messages.Add(message);
        _context.SaveChanges();

        var retrieved = _context.UserSessions.Include(s => s.Messages).First(s => s.Id == session.Id);
        Assert.Single(retrieved.Messages);
        Assert.Equal(retrieved.Id, retrieved.Messages[0].SessionId);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
