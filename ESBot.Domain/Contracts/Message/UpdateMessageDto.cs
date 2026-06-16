using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Message;

public class UpdateMessageDto : IUpdateDto
{
    [MinLength(1)]
    [MaxLength(4000)]
    public string? Content { get; set; }
}