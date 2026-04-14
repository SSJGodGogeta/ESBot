using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class QuizRequestFilter  : IEntityFilter<QuizRequest>
{
    public async Task<List<QuizRequest>> Apply(IQueryable<QuizRequest> query)
    {
        return new List<QuizRequest>();
    }
}