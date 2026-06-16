using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.SubmittedAnswer;

public class UpdateSubmittedAnswerDto : IUpdateDto
{
    [MaxLength(2000)]
    public string? Answer { get; set; }
}