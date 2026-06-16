using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.User;

public class CreateUserDto : ICreateDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [MaxLength(50)]
    public string Password { get; set; } = null!;
}