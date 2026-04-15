using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class MessageEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public MessageEntityTests()
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
    public void Message_Creation_WithValidData_ShouldSucceed()
    {
        // Arrange: create a user and session first
        var user = new User { Username = "muser", Email = "m@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var message = new Message { Session = session, Content = "Hello", Role = EMessageRole.User };

        // Act
        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.Messages.Add(message);
        _context.SaveChanges();

        // Assert
        var saved = _context.Messages.Include(m => m.Session).FirstOrDefault(m => m.Content == "Hello");
        Assert.NotNull(saved);
        Assert.Equal(EMessageRole.User, saved.Role);
        Assert.Equal("Hello", saved.Content);
        Assert.True(saved.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(session.Id, saved.SessionId);
    }

    [Fact]
    public void Message_Validation_MissingContent_ShouldFail()
    {
        var message = new Message { Content = null! };

        var validationContext = new ValidationContext(message);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(message, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Content"));
    }

    [Fact]
    public void Message_Validation_EmptyContent_ShouldFail()
    {
        var message = new Message { Content = string.Empty, Role = EMessageRole.User };

        var validationContext = new ValidationContext(message);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(message, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Message.Content)));
    }

    [Fact]
    public void Message_Validation_ContentLongerThanMaxLength_ShouldFail()
    {
        var message = new Message
        {
            Content = new string('x', 4001),
            Role = EMessageRole.Bot
        };

        var validationContext = new ValidationContext(message);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(message, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Message.Content)));
    }
    
    [Fact]
    public void Message_Validation_ContentAtMaxLength_ShouldSucceed()
    {
        var message = new Message
        {
            Content = new string('x', 4000),
            Role = EMessageRole.User
        };

        var validationContext = new ValidationContext(message);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(message, validationContext, validationResults, true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Message_Relationships_SessionAssociation_ShouldBeConsistent()
    {
        var user = new User { Username = "msguser", Email = "u@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var message = new Message { Content = "Rel test", Role = EMessageRole.Bot };

        // Use helper on session
        session.AddMessage(message);

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.Messages.Add(message);
        _context.SaveChanges();

        var retrieved = _context.UserSessions
            .Include(s => s.Messages)
            .First(s => s.Id == session.Id);

        Assert.Single(retrieved.Messages);
        Assert.Equal(retrieved.Id, retrieved.Messages.First().SessionId);
        Assert.Equal("Rel test", retrieved.Messages.First().Content);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
