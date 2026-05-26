using ESBot.Domain.Entities;

namespace ESBot.Application.Contracts;

public interface ISessionRepository
{
    UserSession CreateSession(UserSession session);

    UserSession? FindSessionById(Guid sessionId);

    IReadOnlyList<UserSession> FindSessionsByUser(Guid userId);

    void AppendMessage(Guid sessionId, Message message);

    IReadOnlyList<Message> GetMessageHistory(Guid sessionId);

    UserSession UpdateSessionEndTime(Guid sessionId, DateTime? endedAt);

    bool DeleteSession(Guid sessionId);

    QuizItem? FindQuizItemById(Guid quizItemId);

    void AddSession(UserSession session);

    void AddMessage(Message message);

    void AddQuizRequest(QuizRequest quizRequest);

    void AddQuizItem(QuizItem quizItem);

    void AddSubmittedAnswer(SubmittedAnswer answer);

    void AddEvaluationResult(EvaluationResult evaluationResult);

    void SaveChanges();
}
