using System.ComponentModel.DataAnnotations;
using ESBot.Domain.Interfaces;

namespace ESBot.Domain.Contracts.Session;

public class UpdateSessionDto : IUpdateDto
{
    public DateTime? EndedAt { get; set; }
    
    [MinLength(1)]
    [MaxLength(4000)]
    public string Title { get; set; } = null!;
}