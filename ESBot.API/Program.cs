using ESBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESBot.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =====================
        // Services
        // =====================
        builder.Services.AddDbContext<EsBotDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // =====================
        // DB Migration + Seed
        // =====================
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EsBotDbContext>();

            db.Database.Migrate();

            DbSeeder.Seed(db);
        }

        // =====================
        // Middleware
        // =====================
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        app.Run();
    }
}