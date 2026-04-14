using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class UserSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }   // FK
    
    public User User { get; set; } = null!;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? EndedAt { get; set; }

    // Navigation Properties
    public List<Message> Messages { get; set; } = new();
    public List<QuizRequest> QuizRequests { get; set; } = new();

    // Helper Method
    public void AddMessage(Message message)
    {
        message.Session = this;
        Messages.Add(message);
    }
}