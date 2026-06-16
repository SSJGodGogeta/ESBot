using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Enums;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.QuizRequest;

public class CreateQuizRequestDto : ICreateDto
{
    [Required]
    public Guid SessionId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Topic { get; set; } = null!;

    [Required]
    public EDifficulty Difficulty { get; set; }
}