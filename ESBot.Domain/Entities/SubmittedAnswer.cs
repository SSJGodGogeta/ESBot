using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents an answer submitted by a user for a specific quiz item.
/// Stores the user's response before evaluation.
/// </summary>
public class SubmittedAnswer : IImmutableProperties
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid QuizItemId { get; set; }

    public QuizItem QuizItem { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Answer { get; set; } = null!;

    [Required]
    public DateTime SubmittedAt { get; private set; } = DateTime.UtcNow;

    public EvaluationResult? EvaluationResult { get; set; }
    
    public IEnumerable<string> GetImmutableProperties()
        => new[] { nameof(SubmittedAt) };
    
}