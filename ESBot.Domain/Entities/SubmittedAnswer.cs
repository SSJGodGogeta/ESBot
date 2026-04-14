using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class SubmittedAnswer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid QuizItemId { get; set; }

    public QuizItem QuizItem { get; set; } = null!;

    [Required]
    public string Answer { get; set; } = null!;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public EvaluationResult? EvaluationResult { get; set; }
}