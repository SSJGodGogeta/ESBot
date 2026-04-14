using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class MessageFilter  : IEntityFilter<Message>
{
    public async Task<List<Message>> Apply(IQueryable<Message> query)
    {
        return new List<Message>();
    }
}