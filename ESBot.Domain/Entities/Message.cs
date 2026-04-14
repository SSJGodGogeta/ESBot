using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Enums;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents a single message within a user session.
/// Messages store the conversational exchange between the user and the ESBot system.
/// </summary>
public class Message : IImmutableProperties
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SessionId { get; set; }

    public UserSession Session { get; set; } = null!;

    [Required]
    [MinLength(1)]
    [MaxLength(4000)]
    public string Content { get; set; } = null!;

    [Required]
    public EMessageRole Role { get; set; }

    [Required]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public IEnumerable<string> GetImmutableProperties()
        => new[] { nameof(CreatedAt) };
}