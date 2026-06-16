using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.User;

public class UserDto : IDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int SessionCount { get; set; }
}