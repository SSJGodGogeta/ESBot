using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using ESBot.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reqnroll;

namespace ESBot.Tests.steps;

[Binding]
public sealed class StepSupportHooks(ScenarioContext scenarioContext)
{
    private SqliteConnection? _connection;
    private EsBotDbContext? _dbContext;

    [BeforeScenario]
    public void BeforeScenario()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<EsBotDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new EsBotDbContext(options);
        _dbContext.Database.EnsureCreated();

        var user = new User
        {
            Username = "Student",
            Email = "student@student.com",
            HashedPassword = "password123"
        };

        var session = new UserSession
        {
            User = user
        };
        
        var quizRequest = new QuizRequest()
        {
            Session = session,
            Topic = "General knowledge",
            Difficulty = EDifficulty.Easy
        };
        
        var quizItem = new QuizItem()
        {
            QuizRequest = quizRequest,
            Question = "What is the capital of France?",
            CorrectAnswer = "Paris"
        };

        var submittedAnswer = new SubmittedAnswer()
        {
            QuizItem = quizItem,
            Answer = "Paris"
        };

        _dbContext.Users.Add(user);
        _dbContext.QuizItems.Add(quizItem);
        _dbContext.SubmittedAnswers.Add(submittedAnswer);
        _dbContext.QuizRequests.Add(quizRequest);
        _dbContext.UserSessions.Add(session);
        _dbContext.SaveChanges();

        scenarioContext.Set(_dbContext);
        scenarioContext.Set(new UserAuthenticationScenarioState());
        scenarioContext.Set(new AnswerCourseQuestionsScenarioState());
        scenarioContext.Set(new EvaluateQuizAnswersState());
        scenarioContext.Set(new StubAiInferenceClient());
        scenarioContext.Set<IAuthenticationUseCase>(new AuthenticationUseCase(_dbContext));
        scenarioContext.Set<IAskCourseQuestionUseCase>(
            new AskCourseQuestionUseCase(_dbContext, scenarioContext.Get<StubAiInferenceClient>()));
        scenarioContext.Set<IEvaluateQuizAnswersUseCase>(new EvaluateQuizAnswersUseCase(_dbContext, scenarioContext.Get<StubAiInferenceClient>()));
        scenarioContext.Set(session.Id, "ActiveSessionId");
        scenarioContext.Set(submittedAnswer.Id, "SubmittedAnswerId");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _dbContext?.Dispose();
        _connection?.Dispose();
    }
}

public sealed class UserAuthenticationScenarioState
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public AuthenticationResult? Result { get; set; }
}

public sealed class AnswerCourseQuestionsScenarioState
{
    public string? Answer { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class EvaluateQuizAnswersState
{
    public string? Feedback { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IAuthenticationUseCase
{
    AuthenticationResult Authenticate(AuthenticationRequest request);
}

public sealed class AuthenticationUseCase(EsBotDbContext dbContext) : IAuthenticationUseCase
{
    public AuthenticationResult Authenticate(AuthenticationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthenticationResult(false, null, "fields cannot be empty");
        }

        var user = dbContext.Users.SingleOrDefault(u =>
            u.Email == request.Email &&
            u.HashedPassword == request.Password);

        if (user is null)
        {
            return new AuthenticationResult(false, null, "invalid credentials");
        }

        return new AuthenticationResult(true, "dashboard", $"Welcome, {user.Username}!");
    }
}

public sealed record AuthenticationRequest(string Email, string Password);

public sealed record AuthenticationResult(bool IsSuccessful, string? DestinationPage, string Message);

public sealed class StubAiInferenceClient
{
    public string NextResponse { get; set; } = string.Empty;

    public string Generate(string prompt)
    {
        if (string.IsNullOrWhiteSpace(NextResponse))
        {
            throw new InvalidOperationException("Mock AI response was not configured for this scenario.");
        }

        return NextResponse;
    }
}

public interface IAskCourseQuestionUseCase
{
    AskCourseQuestionResult Ask(Guid sessionId, string question);
}

public sealed class AskCourseQuestionUseCase(
    EsBotDbContext dbContext,
    StubAiInferenceClient aiClient) : IAskCourseQuestionUseCase
{
    public AskCourseQuestionResult Ask(Guid sessionId, string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return new AskCourseQuestionResult(null, "the question cannot be empty");
        }

        var answer = aiClient.Generate(question);

        dbContext.Messages.Add(new Message
        {
            SessionId = sessionId,
            Content = question,
            Role = EMessageRole.User
        });

        dbContext.Messages.Add(new Message
        {
            SessionId = sessionId,
            Content = answer,
            Role = EMessageRole.Bot
        });

        dbContext.SaveChanges();

        return new AskCourseQuestionResult(answer, null);
    }
}
public sealed record AskCourseQuestionResult(string? Answer, string? ErrorMessage);

public interface IEvaluateQuizAnswersUseCase
{
    EvaluateQuizAnswersResult Evaluate(Guid submittedAnswerId, string question);
}

public sealed class EvaluateQuizAnswersUseCase(
    EsBotDbContext dbContext,
    StubAiInferenceClient aiClient) : IEvaluateQuizAnswersUseCase
{
    public EvaluateQuizAnswersResult Evaluate(Guid submittedAnswerId, string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return new EvaluateQuizAnswersResult(null, "the quiz question cannot be empty");
        }

        var submittedAnswer = dbContext.SubmittedAnswers
            .Include(sa => sa.QuizItem)
            .Single(sa => sa.Id == submittedAnswerId);

        if (submittedAnswer.QuizItem.Question != question)
        {
            return new EvaluateQuizAnswersResult(
                null,
                "the quiz question does not match the submitted answer");
        }

        var feedbackAnswer = aiClient.Generate(question);
        
        return new EvaluateQuizAnswersResult(feedbackAnswer, null);
    }
}

public sealed record EvaluateQuizAnswersResult(string? Feedback, string? ErrorMessage);
