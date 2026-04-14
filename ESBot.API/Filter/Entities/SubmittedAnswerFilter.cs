using ESBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class SubmittedAnswerFilter : IEntityFilter<SubmittedAnswer>
{
    public Guid? Id { get; set; }
    public Guid? QuizItemId { get; set; }
    public string? AnswerLike { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool? IsEvaluated { get; set; }

    public bool IncludeQuizItem { get; set; } = false;
    public bool IncludeEvaluationResult { get; set; } = false;

    public async Task<List<SubmittedAnswer>> Apply(IQueryable<SubmittedAnswer> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(a => a.Id == Id);

        if (QuizItemId.HasValue)
            query = query.Where(a => a.QuizItemId == QuizItemId);

        if (!string.IsNullOrWhiteSpace(AnswerLike))
        {
            var search = AnswerLike.ToLower();
            query = query.Where(a =>
                EF.Functions.Like(a.Answer.ToLower(), $"%{search}%"));
        }

        if (From.HasValue)
            query = query.Where(a => a.SubmittedAt >= From.Value);

        if (To.HasValue)
            query = query.Where(a => a.SubmittedAt <= To.Value);

        if (IsEvaluated.HasValue)
        {
            if (IsEvaluated.Value)
                query = query.Where(a => a.EvaluationResult != null);
            else
                query = query.Where(a => a.EvaluationResult == null);
        }

        // =====================
        // INCLUDE
        // =====================

        if (IncludeQuizItem)
            query = query.Include(a => a.QuizItem);

        if (IncludeEvaluationResult)
            query = query.Include(a => a.EvaluationResult);

        // =====================
        // SORTING (neueste zuerst sinnvoll)
        // =====================

        query = query.OrderByDescending(a => a.SubmittedAt);

        // =====================
        // EXECUTION
        // =====================

        return await query.ToListAsync();
    }
}