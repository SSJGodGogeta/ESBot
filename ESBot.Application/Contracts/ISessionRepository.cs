using ESBot.Domain.Entities;

namespace ESBot.Application.Contracts;

public interface ISessionRepository
{
    UserSession? FindSessionById(Guid sessionId);

    QuizItem? FindQuizItemById(Guid quizItemId);

    void AddSession(UserSession session);

    void AddMessage(Message message);

    void AddQuizRequest(QuizRequest quizRequest);

    void AddQuizItem(QuizItem quizItem);

    void AddSubmittedAnswer(SubmittedAnswer answer);

    void AddEvaluationResult(EvaluationResult evaluationResult);

    void SaveChanges();
}
