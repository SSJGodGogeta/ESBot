using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents a single question within a quiz.
/// Each quiz item contains the question text and the correct answer.
/// </summary>
public class QuizItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid QuizRequestId { get; set; }

    public QuizRequest QuizRequest { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Question { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string CorrectAnswer { get; set; } = null!;

    public ICollection<SubmittedAnswer> SubmittedAnswers { get; set; }
        = new List<SubmittedAnswer>();
}