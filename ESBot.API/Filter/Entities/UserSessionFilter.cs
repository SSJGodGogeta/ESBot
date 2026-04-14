using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class UserSessionFilter  : IEntityFilter<UserSession>
{
    public async Task<List<UserSession>> Apply(IQueryable<UserSession> query)
    {
        return new List<UserSession>();
    }
}