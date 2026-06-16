using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.User;

public class UpdateUserDto : IUpdateDto
{
    [MaxLength(50)]
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [MinLength(8)]
    [MaxLength(50)]
    public string? Password { get; set; }
}