using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class SubmittedAnswerEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public SubmittedAnswerEntityTests()
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
    public void SubmittedAnswer_Creation_WithValidData_ShouldSucceed()
    {
        var user = new User { Username = "sauser", Email = "sa@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Geo", Difficulty = EDifficulty.Easy };
        var item = new QuizItem { QuizRequest = request, Question = "Where?", CorrectAnswer = "Here" };
        var answer = new SubmittedAnswer { QuizItem = item, Answer = "Here" };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SubmittedAnswers.Add(answer);
        _context.SaveChanges();

        var saved = _context.SubmittedAnswers.Include(sa => sa.QuizItem).First(sa => sa.Id == answer.Id);
        Assert.Equal("Here", saved.Answer);
        Assert.Equal(item.Id, saved.QuizItemId);
    }

    [Fact]
    public void SubmittedAnswer_Validation_MissingAnswer_ShouldFail()
    {
        var answer = new SubmittedAnswer { Answer = null! };

        var validationContext = new ValidationContext(answer);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(answer, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Answer"));
    }

    [Fact]
    public void Saving_SubmittedAnswer_Without_QuizItem_ShouldFailDbConstraint()
    {
        var answer = new SubmittedAnswer { Answer = "NoItem" };
        _context.SubmittedAnswers.Add(answer);

        Assert.Throws<DbUpdateException>(() => _context.SaveChanges());
    }

    [Fact]
    public void SubmittedAnswer_Validation_AnswerLongerThanMaxLength_ShouldFail()
    {
        var answer = new SubmittedAnswer { Answer = new string('a', 2001) };

        var validationContext = new ValidationContext(answer);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(answer, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(SubmittedAnswer.Answer)));
    }

    [Fact]
    public void SubmittedAnswer_Creation_WithoutEvaluationResult_ShouldSucceed()
    {
        var user = new User { Username = "optional-eval", Email = "optional-eval@example.com", HashedPassword = "password" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Geo", Difficulty = EDifficulty.Easy };
        var item = new QuizItem { QuizRequest = request, Question = "Where?", CorrectAnswer = "Here" };
        var answer = new SubmittedAnswer { QuizItem = item, Answer = "Here" };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SubmittedAnswers.Add(answer);
        _context.SaveChanges();

        var saved = _context.SubmittedAnswers.Include(sa => sa.EvaluationResult).First(sa => sa.Id == answer.Id);
        Assert.Null(saved.EvaluationResult);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
