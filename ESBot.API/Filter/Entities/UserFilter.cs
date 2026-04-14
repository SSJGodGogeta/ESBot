using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class UserFilter  : IEntityFilter<User>
{
    public async Task<List<User>> Apply(IQueryable<User> query)
    {
        return new List<User>();
    }
}