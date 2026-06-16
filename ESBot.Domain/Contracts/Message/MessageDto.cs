using ESBot.Domain.Enums;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Message;

public class MessageDto :  IDto
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public string Content { get; set; } = null!;

    public EMessageRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
}