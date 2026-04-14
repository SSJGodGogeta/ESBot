using ESBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class QuizItemFilter : IEntityFilter<QuizItem>
{
    public Guid? Id { get; set; }
    public Guid? QuizRequestId { get; set; }
    public string? QuestionLike { get; set; }
    public string? CorrectAnswerLike { get; set; }
    public bool? HasSubmittedAnswers { get; set; }

    public bool IncludeSubmittedAnswers { get; set; } = false;
    public bool IncludeQuizRequest { get; set; } = false;

    public async Task<List<QuizItem>> Apply(IQueryable<QuizItem> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(q => q.Id == Id);

        if (QuizRequestId.HasValue)
            query = query.Where(q => q.QuizRequestId == QuizRequestId);

        if (!string.IsNullOrWhiteSpace(QuestionLike))
        {
            var search = QuestionLike.ToLower();
            query = query.Where(q =>
                EF.Functions.Like(q.Question.ToLower(), $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(CorrectAnswerLike))
        {
            var search = CorrectAnswerLike.ToLower();
            query = query.Where(q =>
                EF.Functions.Like(q.CorrectAnswer.ToLower(), $"%{search}%"));
        }

        if (HasSubmittedAnswers.HasValue)
        {
            if (HasSubmittedAnswers.Value)
                query = query.Where(q => q.SubmittedAnswers.Any());
            else
                query = query.Where(q => !q.SubmittedAnswers.Any());
        }

        // =====================
        // INCLUDE
        // =====================

        if (IncludeSubmittedAnswers)
            query = query.Include(q => q.SubmittedAnswers);

        if (IncludeQuizRequest)
            query = query.Include(q => q.QuizRequest);

        // =====================
        // EXECUTION
        // =====================

        return await query.ToListAsync();
    }
}