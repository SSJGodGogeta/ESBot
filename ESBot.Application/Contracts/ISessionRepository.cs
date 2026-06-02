using ESBot.Domain.Entities;

namespace ESBot.Application.Contracts;

public interface ISessionRepository
{
    Session CreateSession(Session session);

    Session? FindSessionById(Guid sessionId);

    IReadOnlyList<Session> FindSessionsByUser(Guid userId);

    void AppendMessage(Guid sessionId, Message message);

    IReadOnlyList<Message> GetMessageHistory(Guid sessionId);

    Session UpdateSessionEndTime(Guid sessionId, DateTime? endedAt);

    bool DeleteSession(Guid sessionId);

    QuizItem? FindQuizItemById(Guid quizItemId);

    void AddSession(Session session);

    void AddMessage(Message message);

    void AddQuizRequest(QuizRequest quizRequest);

    void AddQuizItem(QuizItem quizItem);

    void AddSubmittedAnswer(SubmittedAnswer answer);

    void AddEvaluationResult(EvaluationResult evaluationResult);

    void SaveChanges();
}
