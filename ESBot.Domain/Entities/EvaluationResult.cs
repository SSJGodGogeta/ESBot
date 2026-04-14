using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents the evaluation result of a submitted quiz answer.
/// Contains correctness, score, and optional feedback for the user.
/// </summary>
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