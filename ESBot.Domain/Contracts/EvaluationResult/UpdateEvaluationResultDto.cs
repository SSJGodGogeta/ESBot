using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.EvaluationResult;

public class UpdateEvaluationResultDto :  IUpdateDto
{
    public bool? IsCorrect { get; set; }

    public double? Score { get; set; }

    [MaxLength(2000)]
    public string? Feedback { get; set; }
}