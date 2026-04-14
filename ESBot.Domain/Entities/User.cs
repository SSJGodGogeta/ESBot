using System.ComponentModel.DataAnnotations;

namespace ESBot.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string HashedPassword { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<UserSession> Sessions { get; set; } = new();
}