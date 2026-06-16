using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.QuizItem;

public class UpdateQuizItemDto :  IUpdateDto
{
    [MaxLength(2000)]
    public string? Question { get; set; }

    [MaxLength(1000)]
    public string? CorrectAnswer { get; set; }
}