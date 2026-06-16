using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Session;

public class UpdateSessionDto : IUpdateDto
{
    public DateTime? EndedAt { get; set; }
}