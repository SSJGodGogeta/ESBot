using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.QuizItem;

public class CreateQuizItemDto : ICreateDto
{
    [Required]
    public Guid QuizRequestId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Question { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string CorrectAnswer { get; set; } = null!;
}