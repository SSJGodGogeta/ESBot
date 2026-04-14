using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Enums;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents a request to generate a quiz for a specific topic within a user session.
/// A quiz request contains metadata such as topic and difficulty.
/// </summary>
public class QuizRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SessionId { get; set; }

    public UserSession Session { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Topic { get; set; } = null!;

    [Required]
    public EDifficulty Difficulty { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizItem> QuizItems { get; set; }
        = new List<QuizItem>();
}