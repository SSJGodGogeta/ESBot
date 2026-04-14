using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;

namespace ESBot.Tests;

public class EvaluationResultEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly EsBotDbContext _context;

    public EvaluationResultEntityTests()
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
    public void EvaluationResult_Creation_WithValidData_ShouldSucceed()
    {
        var user = new User { Username = "eruser", Email = "er@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Sci", Difficulty = EDifficulty.Hard };
        var item = new QuizItem { QuizRequest = request, Question = "Why?", CorrectAnswer = "Because" };
        var answer = new SubmittedAnswer { QuizItem = item, Answer = "Because" };
        var eval = new EvaluationResult { SubmittedAnswer = answer, IsCorrect = true, Score = 1.0, Feedback = "Good" };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SubmittedAnswers.Add(answer);
        _context.EvaluationResults.Add(eval);
        _context.SaveChanges();

        var saved = _context.EvaluationResults.Include(e => e.SubmittedAnswer).First(e => e.Id == eval.Id);
        Assert.True(saved.IsCorrect);
        Assert.Equal(1.0, saved.Score);
        Assert.Equal("Good", saved.Feedback);
        Assert.Equal(answer.Id, saved.SubmittedAnswerId);
    }

    [Fact]
    public void Saving_EvaluationResult_Without_SubmittedAnswer_ShouldFailDbConstraint()
    {
        // Saving an EvaluationResult that references no existing SubmittedAnswer should fail at the DB level (FK constraint)
        var eval = new EvaluationResult { IsCorrect = false, Score = 0.0 };
        _context.EvaluationResults.Add(eval);

        Assert.Throws<DbUpdateException>(() => _context.SaveChanges());
    }

    [Fact]
    public void EvaluationResult_Creation_WithoutFeedback_ShouldSucceed()
    {
        var user = new User { Username = "sieuser", Email = "sie@example.com", HashedPassword = "pw" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Sci", Difficulty = EDifficulty.Hard };
        var item = new QuizItem { QuizRequest = request, Question = "Why?", CorrectAnswer = "Because" };
        var answer = new SubmittedAnswer { QuizItem = item, Answer = "Because" };
        var eval = new EvaluationResult { SubmittedAnswer = answer, IsCorrect = false, Score = 0.25, Feedback = null };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SubmittedAnswers.Add(answer);
        _context.EvaluationResults.Add(eval);
        _context.SaveChanges();

        var saved = _context.EvaluationResults.Include(e => e.SubmittedAnswer).First(e => e.Id == eval.Id);
        Assert.False(saved.IsCorrect);
        Assert.Equal(0.25, saved.Score);
        Assert.Null(saved.Feedback);
    }

    [Fact]
    public void EvaluationResult_Creation_WithNegativeScore_ShouldSucceed()
    {
        var user = new User { Username = "negscoreuser", Email = "negscore@example.com", HashedPassword = "password" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Sci", Difficulty = EDifficulty.Hard };
        var item = new QuizItem { QuizRequest = request, Question = "Why?", CorrectAnswer = "Because" };
        var answer = new SubmittedAnswer { QuizItem = item, Answer = "Because" };
        var eval = new EvaluationResult { SubmittedAnswer = answer, IsCorrect = false, Score = -1.0, Feedback = "Penalty" };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SubmittedAnswers.Add(answer);
        _context.EvaluationResults.Add(eval);
        _context.SaveChanges();

        var saved = _context.EvaluationResults.First(e => e.Id == eval.Id);
        Assert.Equal(-1.0, saved.Score);
    }

    [Fact]
    public void EvaluationResult_Creation_WithScoreGreaterThanOne_ShouldSucceed()
    {
        var user = new User { Username = "highscoreuser", Email = "highscore@example.com", HashedPassword = "password" };
        var session = new UserSession { User = user };
        var request = new QuizRequest { Session = session, Topic = "Sci", Difficulty = EDifficulty.Hard };
        var item = new QuizItem { QuizRequest = request, Question = "Why?", CorrectAnswer = "Because" };
        var answer = new SubmittedAnswer { QuizItem = item, Answer = "Because" };
        var eval = new EvaluationResult { SubmittedAnswer = answer, IsCorrect = true, Score = 1.5, Feedback = "Bonus" };

        _context.Users.Add(user);
        _context.UserSessions.Add(session);
        _context.QuizRequests.Add(request);
        _context.QuizItems.Add(item);
        _context.SubmittedAnswers.Add(answer);
        _context.EvaluationResults.Add(eval);
        _context.SaveChanges();

        var saved = _context.EvaluationResults.First(e => e.Id == eval.Id);
        Assert.Equal(1.5, saved.Score);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
}
