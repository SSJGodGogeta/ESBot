using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.SubmittedAnswer;

public class SubmittedAnswerDto : IDto
{
    public Guid Id { get; set; }

    public Guid QuizItemId { get; set; }

    public string Answer { get; set; } = null!;

    public DateTime SubmittedAt { get; set; }

    public bool IsEvaluated { get; set; }
}