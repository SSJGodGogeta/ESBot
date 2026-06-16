using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.QuizItem;

public class QuizItemDto :  IDto
{
    public Guid Id { get; set; }

    public Guid QuizRequestId { get; set; }

    public string Question { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public int SubmittedAnswerCount { get; set; }
}