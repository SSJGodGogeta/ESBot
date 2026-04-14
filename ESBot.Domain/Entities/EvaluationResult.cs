using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class EvaluationResult
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SubmittedAnswerId { get; set; }

    public SubmittedAnswer SubmittedAnswer { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public double Score { get; set; }

    public string? Feedback { get; set; }
}