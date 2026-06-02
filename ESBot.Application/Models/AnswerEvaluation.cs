namespace ESBot.Application.Models;

public record AnswerEvaluation(bool IsCorrect, double Score, string? Feedback);
