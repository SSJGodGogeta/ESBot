using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class UserFilter  : IEntityFilter<User>
{
    public async Task<List<User>> Apply(IQueryable<User> query)
    {
        User arman  = new User();
        arman.Username = "Arman";
        arman.Email = "arman@arman.com";
        List<User>  users = new List<User>();
        users.Add(arman);

        return users;
    }
}