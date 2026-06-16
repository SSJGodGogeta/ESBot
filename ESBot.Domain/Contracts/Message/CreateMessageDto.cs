using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Enums;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Message;

public class CreateMessageDto : ICreateDto
{
    [Required]
    public Guid SessionId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(4000)]
    public string Content { get; set; } = null!;

    [Required]
    public EMessageRole Role { get; set; }
}