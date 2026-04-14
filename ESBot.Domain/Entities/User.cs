using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents a registered user in the ESBot system.
/// A user is the root identity entity and can have multiple learning sessions.
/// </summary>
public class User : IImmutableProperties
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [MaxLength(50)]
    public string HashedPassword { get; set; } = null!;
    
    [Required]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    
    public IEnumerable<string> GetImmutableProperties()
        => new[] { nameof(CreatedAt) };
}