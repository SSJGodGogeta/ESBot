using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class QuizItemEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public QuizItemEntityTests()
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
    public void QuizItem_Creation_WithValidData_ShouldSucceed()
    {
        var user = new User { Username = "qiuser", Email = "qi@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "History", Difficulty = EDifficulty.Medium };
        var item = new QuizItem { QuizRequest = request, Question = "Q?", CorrectAnswer = "A" };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SaveChanges();

        var saved = _context.QuizItems.Include(i => i.QuizRequest).First(i => i.Id == item.Id);
        Assert.Equal("Q?", saved.Question);
        Assert.Equal("A", saved.CorrectAnswer);
        Assert.Equal(request.Id, saved.QuizRequestId);
    }

    [Fact]
    public void QuizItem_Validation_MissingQuestion_ShouldFail()
    {
        var item = new QuizItem { Question = null!, CorrectAnswer = "A" };

        var validationContext = new ValidationContext(item);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(item, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Question"));
    }

    [Fact]
    public void QuizItem_Validation_MissingCorrectAnswer_ShouldFail()
    {
        var item = new QuizItem { Question = "What is tested?", CorrectAnswer = null! };

        var validationContext = new ValidationContext(item);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(item, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(QuizItem.CorrectAnswer)));
    }

    [Fact]
    public void QuizItem_Validation_QuestionLongerThanMaxLength_ShouldFail()
    {
        var item = new QuizItem
        {
            Question = new string('q', 2001),
            CorrectAnswer = "A"
        };

        var validationContext = new ValidationContext(item);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(item, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(QuizItem.Question)));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
