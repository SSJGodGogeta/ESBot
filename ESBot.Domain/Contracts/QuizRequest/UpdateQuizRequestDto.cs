using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Enums;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.QuizRequest;

public class UpdateQuizRequestDto : IUpdateDto
{
    [MaxLength(200)]
    public string? Topic { get; set; }

    public EDifficulty? Difficulty { get; set; }
}