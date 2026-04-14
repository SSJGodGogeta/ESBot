using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

/// <summary>
/// Represents a registered user in the ESBot system.
/// A user is the root identity entity and can have multiple learning sessions.
/// </summary>
public class User
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
}