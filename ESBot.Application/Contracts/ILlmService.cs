using ESBot.Application.Models;
using ESBot.Domain.Enums;

namespace ESBot.Application.Contracts;

public interface ILlmService
{
    string GenerateResponse(string message);

    IReadOnlyList<GeneratedQuizItem> GenerateQuiz(string topic, EDifficulty difficulty, int questionCount);

    AnswerEvaluation EvaluateAnswer(string question, string correctAnswer, string submittedAnswer);
}
