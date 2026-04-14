using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents a single learning session of a user.
/// A session groups all messages, quiz requests, and interactions within a specific learning context.
/// </summary>
public class UserSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    [Required]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? EndedAt { get; set; }

    public ICollection<Message> Messages { get; set; }
        = new List<Message>();

    public ICollection<QuizRequest> QuizRequests { get; set; }
        = new List<QuizRequest>();

    public void AddMessage(Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        message.Session = this;
        Messages.Add(message);
    }
}