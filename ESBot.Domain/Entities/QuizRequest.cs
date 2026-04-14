using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class QuizRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SessionId { get; set; }

    public UserSession Session { get; set; } = null!;

    [Required]
    public string Topic { get; set; } = null!;

    public EDifficulty Difficulty { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<QuizItem> Items { get; set; } = new();
}