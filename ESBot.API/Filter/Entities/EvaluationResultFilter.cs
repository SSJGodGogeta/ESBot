using ESBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class EvaluationResultFilter : IEntityFilter<EvaluationResult>
{
    public Guid? Id { get; set; }
    public Guid? SubmittedAnswerId { get; set; }
    public bool? IsCorrect { get; set; }
    public double? MinScore { get; set; }
    public double? MaxScore { get; set; }
    public string? FeedbackLike { get; set; }

    public bool IncludeSubmittedAnswer { get; set; } = false;

    public async Task<List<EvaluationResult>> Apply(IQueryable<EvaluationResult> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(e => e.Id == Id);

        if (SubmittedAnswerId.HasValue)
            query = query.Where(e => e.SubmittedAnswerId == SubmittedAnswerId);

        if (IsCorrect.HasValue)
            query = query.Where(e => e.IsCorrect == IsCorrect);

        if (MinScore.HasValue)
            query = query.Where(e => e.Score >= MinScore);

        if (MaxScore.HasValue)
            query = query.Where(e => e.Score <= MaxScore);

        if (!string.IsNullOrWhiteSpace(FeedbackLike))
            query = query.Where(e =>
                EF.Functions.Like(e.Feedback!.ToLower(), $"%{FeedbackLike.ToLower()}%"));

        // =====================
        // INCLUDE
        // =====================

        if (IncludeSubmittedAnswer)
            query = query.Include(e => e.SubmittedAnswer);

        var results = await query.ToListAsync();

        return results;
    }
}