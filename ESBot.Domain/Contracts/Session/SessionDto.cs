using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Session;

public class SessionDto : IDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    
    public string Title { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int MessageCount { get; set; }

    public int QuizRequestCount { get; set; }
}