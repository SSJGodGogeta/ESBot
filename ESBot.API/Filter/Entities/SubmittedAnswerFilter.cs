using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class SubmittedAnswerFilter  : IEntityFilter<SubmittedAnswer>
{
    public async Task<List<SubmittedAnswer>> Apply(IQueryable<SubmittedAnswer> query)
    {
        return new List<SubmittedAnswer>();
    }
}