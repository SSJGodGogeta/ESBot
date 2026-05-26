using ESBot.Application.Contracts;
using ESBot.Domain.Entities;
using ESBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESBot.Infrastructure.Repositories;

public class SessionRepository(EsBotDbContext context) : ISessionRepository
{
    public UserSession CreateSession(UserSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        context.UserSessions.Add(session);
        context.SaveChanges();
        return session;
    }

    public UserSession? FindSessionById(Guid sessionId)
        => context.UserSessions.SingleOrDefault(session => session.Id == sessionId);

    public IReadOnlyList<UserSession> FindSessionsByUser(Guid userId)
        => context.UserSessions
            .Where(session => session.UserId == userId)
            .OrderByDescending(session => session.StartedAt)
            .ToList();

    public void AppendMessage(Guid sessionId, Message message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var session = FindRequiredSession(sessionId);
        message.SessionId = session.Id;
        session.AddMessage(message);

        context.Messages.Add(message);
        context.SaveChanges();
    }

    public IReadOnlyList<Message> GetMessageHistory(Guid sessionId)
        => context.Messages
            .Where(message => message.SessionId == sessionId)
            .OrderBy(message => message.CreatedAt)
            .ToList();

    public UserSession UpdateSessionEndTime(Guid sessionId, DateTime? endedAt)
    {
        var session = FindRequiredSession(sessionId);
        session.EndedAt = endedAt;
        context.SaveChanges();
        return session;
    }

    public bool DeleteSession(Guid sessionId)
    {
        var session = FindSessionById(sessionId);
        if (session is null)
            return false;

        context.UserSessions.Remove(session);
        context.SaveChanges();
        return true;
    }

    public QuizItem? FindQuizItemById(Guid quizItemId)
        => context.QuizItems
            .Include(item => item.QuizRequest)
            .SingleOrDefault(item => item.Id == quizItemId);

    public void AddSession(UserSession session)
        => context.UserSessions.Add(session);

    public void AddMessage(Message message)
        => context.Messages.Add(message);

    public void AddQuizRequest(QuizRequest quizRequest)
        => context.QuizRequests.Add(quizRequest);

    public void AddQuizItem(QuizItem quizItem)
        => context.QuizItems.Add(quizItem);

    public void AddSubmittedAnswer(SubmittedAnswer answer)
        => context.SubmittedAnswers.Add(answer);

    public void AddEvaluationResult(EvaluationResult evaluationResult)
        => context.EvaluationResults.Add(evaluationResult);

    public void SaveChanges()
        => context.SaveChanges();

    private UserSession FindRequiredSession(Guid sessionId)
        => FindSessionById(sessionId)
            ?? throw new KeyNotFoundException($"Session '{sessionId}' was not found.");
}
