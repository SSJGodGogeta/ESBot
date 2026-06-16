using ESBot.Domain.Enums;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.QuizRequest;

public class QuizRequestDto : IDto
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public string Topic { get; set; } = null!;

    public EDifficulty Difficulty { get; set; }

    public DateTime CreatedAt { get; set; }

    public int QuizItemCount { get; set; }
}