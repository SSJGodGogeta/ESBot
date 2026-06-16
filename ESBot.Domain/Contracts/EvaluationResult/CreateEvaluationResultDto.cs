using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.EvaluationResult;

public class CreateEvaluationResultDto :  ICreateDto
{
    [Required]
    public Guid SubmittedAnswerId { get; set; }

    [Required]
    public bool IsCorrect { get; set; }

    [Required]
    public double Score { get; set; }

    [MaxLength(2000)]
    public string? Feedback { get; set; }
}