using ESBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API.Filter.Entities;

public class UserFilter : IEntityFilter<User>
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? UsernameLike { get; set; }
    public string? Email { get; set; }
    public string? EmailLike { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public bool IncludeSessions { get; set; } = false;

    public async Task<List<User>> Apply(IQueryable<User> query)
    {
        // =====================
        // FILTERS
        // =====================

        if (Id.HasValue)
            query = query.Where(u => u.Id == Id);

        if (!string.IsNullOrWhiteSpace(Username))
            query = query.Where(u => u.Username == Username);

        if (!string.IsNullOrWhiteSpace(UsernameLike))
        {
            var search = UsernameLike.ToLower();
            query = query.Where(u =>
                EF.Functions.Like(u.Username.ToLower(), $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(Email))
            query = query.Where(u => u.Email == Email);

        if (!string.IsNullOrWhiteSpace(EmailLike))
        {
            var search = EmailLike.ToLower();
            query = query.Where(u =>
                EF.Functions.Like(u.Email.ToLower(), $"%{search}%"));
        }

        if (From.HasValue)
            query = query.Where(u => u.CreatedAt >= From.Value);

        if (To.HasValue)
            query = query.Where(u => u.CreatedAt <= To.Value);

        // =====================
        // INCLUDE
        // =====================

        if (IncludeSessions)
            query = query.Include(u => u.Sessions);

        // =====================
        // SORTING
        // =====================

        query = query.OrderBy(u => u.Username);

        // =====================
        // EXECUTION
        // =====================

        return await query.ToListAsync();
    }
}