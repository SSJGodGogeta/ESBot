using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class QuizItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid QuizRequestId { get; set; }

    public QuizRequest QuizRequest { get; set; } = null!;

    [Required]
    public string Question { get; set; } = null!;

    [Required]
    public string CorrectAnswer { get; set; } = null!;

    // Navigation
    public List<SubmittedAnswer> SubmittedAnswers { get; set; } = new();
}