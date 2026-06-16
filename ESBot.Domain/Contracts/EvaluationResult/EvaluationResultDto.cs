using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.EvaluationResult;

public class EvaluationResultDto :  IDto
{
    public Guid Id { get; set; }

    public Guid SubmittedAnswerId { get; set; }

    public bool IsCorrect { get; set; }

    public double Score { get; set; }

    public string? Feedback { get; set; }
}