using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SessionId { get; set; }

    public UserSession Session { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public string Content { get; set; } = null!;

    [Required]
    public EMessageRole Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}