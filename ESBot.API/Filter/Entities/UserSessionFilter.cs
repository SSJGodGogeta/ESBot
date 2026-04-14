using ESBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class UserSessionFilter : IEntityFilter<UserSession>
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? StartedFrom { get; set; }
    public DateTime? StartedTo { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasMessages { get; set; }
    public bool? HasQuizRequests { get; set; }

    public bool IncludeUser { get; set; } = false;
    public bool IncludeMessages { get; set; } = false;
    public bool IncludeQuizRequests { get; set; } = false;

    public async Task<List<UserSession>> Apply(IQueryable<UserSession> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(s => s.Id == Id);

        if (UserId.HasValue)
            query = query.Where(s => s.UserId == UserId);

        if (StartedFrom.HasValue)
            query = query.Where(s => s.StartedAt >= StartedFrom.Value);

        if (StartedTo.HasValue)
            query = query.Where(s => s.StartedAt <= StartedTo.Value);

        if (IsActive.HasValue)
        {
            if (IsActive.Value)
                query = query.Where(s => s.EndedAt == null);
            else
                query = query.Where(s => s.EndedAt != null);
        }

        if (HasMessages.HasValue)
        {
            if (HasMessages.Value)
                query = query.Where(s => s.Messages.Any());
            else
                query = query.Where(s => !s.Messages.Any());
        }

        if (HasQuizRequests.HasValue)
        {
            if (HasQuizRequests.Value)
                query = query.Where(s => s.QuizRequests.Any());
            else
                query = query.Where(s => !s.QuizRequests.Any());
        }

        // =====================
        // INCLUDE
        // =====================

        if (IncludeUser)
            query = query.Include(s => s.User);

        if (IncludeMessages)
            query = query.Include(s => s.Messages);

        if (IncludeQuizRequests)
            query = query.Include(s => s.QuizRequests);

        // =====================
        // SORTING (sehr wichtig für UI)
        // =====================

        query = query.OrderByDescending(s => s.StartedAt);

        // =====================
        // EXECUTION
        // =====================

        return await query.ToListAsync();
    }
}