using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.SubmittedAnswer;

public class CreateSubmittedAnswerDto : ICreateDto
{
    [Required]
    public Guid QuizItemId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Answer { get; set; } = null!;
}