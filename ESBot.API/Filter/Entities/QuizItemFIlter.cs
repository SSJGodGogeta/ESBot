using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class QuizItemFilter  : IEntityFilter<QuizItem>
{
    public async Task<List<QuizItem>> Apply(IQueryable<QuizItem> query)
    {
        return new List<QuizItem>();
    }
}