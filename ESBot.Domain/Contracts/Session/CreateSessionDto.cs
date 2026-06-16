using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Session;

public class CreateSessionDto : ICreateDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(4000)]
    public string Title { get; set; } = null!;
}