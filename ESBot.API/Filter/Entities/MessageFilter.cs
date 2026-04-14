using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class MessageFilter : IEntityFilter<Message>
{
    public Guid? Id { get; set; }
    public Guid? SessionId { get; set; }
    public EMessageRole? Role { get; set; }
    public string? ContentLike { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public bool IncludeSession { get; set; } = false;

    public async Task<List<Message>> Apply(IQueryable<Message> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(m => m.Id == Id);

        if (SessionId.HasValue)
            query = query.Where(m => m.SessionId == SessionId);

        if (Role.HasValue)
            query = query.Where(m => m.Role == Role);

        if (!string.IsNullOrWhiteSpace(ContentLike))
        {
            var search = ContentLike.ToLower();
            query = query.Where(m =>
                EF.Functions.Like(m.Content.ToLower(), $"%{search}%"));
        }

        if (From.HasValue)
            query = query.Where(m => m.CreatedAt >= From.Value);

        if (To.HasValue)
            query = query.Where(m => m.CreatedAt <= To.Value);

        // =====================
        // INCLUDE
        // =====================

        if (IncludeSession)
            query = query.Include(m => m.Session);

        // =====================
        // SORTING (sehr wichtig für Chat!)
        // =====================

        query = query.OrderBy(m => m.CreatedAt);

        // =====================
        // EXECUTION
        // =====================

        return await query.ToListAsync();
    }
}