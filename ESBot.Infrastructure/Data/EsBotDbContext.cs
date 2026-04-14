namespace ESBot.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using ESBot.Domain.Entities;

public class EsBotDbContext : DbContext
{
    public EsBotDbContext(DbContextOptions<EsBotDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<QuizRequest> QuizRequests { get; set; }
    public DbSet<QuizItem> QuizItems { get; set; }
    public DbSet<SubmittedAnswer> SubmittedAnswers { get; set; }
    public DbSet<EvaluationResult> EvaluationResults { get; set; }
}