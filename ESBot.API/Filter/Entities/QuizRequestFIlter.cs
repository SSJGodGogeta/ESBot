using ESBot.Domain.Entities;
using ESBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class QuizRequestFilter : IEntityFilter<QuizRequest>
{
    public Guid? Id { get; set; }
    public Guid? SessionId { get; set; }
    public EDifficulty? Difficulty { get; set; }
    public string? TopicLike { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool? HasItems { get; set; }

    public bool IncludeSession { get; set; } = false;
    public bool IncludeItems { get; set; } = false;

    public async Task<List<QuizRequest>> Apply(IQueryable<QuizRequest> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(q => q.Id == Id);

        if (SessionId.HasValue)
            query = query.Where(q => q.SessionId == SessionId);

        if (Difficulty.HasValue)
            query = query.Where(q => q.Difficulty == Difficulty);

        if (!string.IsNullOrWhiteSpace(TopicLike))
        {
            var search = TopicLike.ToLower();
            query = query.Where(q =>
                EF.Functions.Like(q.Topic.ToLower(), $"%{search}%"));
        }

        if (From.HasValue)
            query = query.Where(q => q.CreatedAt >= From.Value);

        if (To.HasValue)
            query = query.Where(q => q.CreatedAt <= To.Value);

        if (HasItems.HasValue)
        {
            if (HasItems.Value)
                query = query.Where(q => q.QuizItems.Any());
            else
                query = query.Where(q => !q.QuizItems.Any());
        }

        // =====================
        // INCLUDE
        // =====================

        if (IncludeSession)
            query = query.Include(q => q.Session);

        if (IncludeItems)
            query = query.Include(q => q.QuizItems);

        // =====================
        // SORTING (sehr sinnvoll)
        // =====================

        query = query.OrderByDescending(q => q.CreatedAt);

        // =====================
        // EXECUTION
        // =====================

        return await query.ToListAsync();
    }
}