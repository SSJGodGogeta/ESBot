using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class QuizRequestEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public QuizRequestEntityTests()
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
    public void QuizRequest_Creation_WithValidData_ShouldSucceed()
    {
        var user = new User { Username = "qruser", Email = "qr@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Math", Difficulty = EDifficulty.Easy };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.SaveChanges();

        var saved = _context.QuizRequests.Include(q => q.Session).First(q => q.Id == request.Id);
        Assert.Equal("Math", saved.Topic);
        Assert.Equal(EDifficulty.Easy, saved.Difficulty);
        Assert.Equal(session.Id, saved.SessionId);
    }

    [Fact]
    public void QuizRequest_Validation_MissingTopic_ShouldFail()
    {
        var request = new QuizRequest { Topic = null! };

        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Topic"));
    }

    [Fact]
    public void QuizRequest_Validation_TopicLongerThanMaxLength_ShouldFail()
    {
        var request = new QuizRequest
        {
            Topic = new string('t', 201),
            Difficulty = EDifficulty.Medium
        };

        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(QuizRequest.Topic)));
    }

    [Fact]
    public void QuizRequest_Validation_TopicAtMaxLength_ShouldSucceed()
    {
        var request = new QuizRequest
        {
            Topic = new string('t', 200),
            Difficulty = EDifficulty.Medium
        };

        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void QuizRequest_Relationships_QuizItemsAssociation_ShouldBeConsistent()
    {
        // Verifies the one-to-many EF relationship: the parent QuizRequest must expose the child QuizItem
        // through its QuizItems collection, and the child must reference the same parent via QuizRequestId.
        var user = new User { Username = "qreluser", Email = "qrel@example.com", HashedPassword = "password" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Math", Difficulty = EDifficulty.Easy };
        var item = new QuizItem { QuizRequest = request, Question = "2+2?", CorrectAnswer = "4" };

        request.QuizItems.Add(item);

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SaveChanges();

        var saved = _context.QuizRequests
            .Include(q => q.QuizItems)
            .First(q => q.Id == request.Id);
        
        Assert.Single(saved.QuizItems);
        Assert.Equal(request.Id, saved.QuizItems.First().QuizRequestId);
        Assert.Equal("2+2?", saved.QuizItems.First().Question);
    }
    
    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
