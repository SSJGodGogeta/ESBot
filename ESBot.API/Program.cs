using System.Reflection;
using ESBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "ESBot API",
                Version = "v1",
                Contact = new OpenApiContact()
                {
                    Name = "Armanbir Singh",
                    Email = "arsiit01@hs-esslingen.de"
                },
                Description = "ESBot is an AI-powered learning assistant designed for structured and interactive learning experiences. " +
                              "It enables students to engage in conversational learning sessions, ask context-based questions, " +
                              "generate quizzes, and receive automated feedback on their answers. " +
                              "The backend manages persistent user sessions, stores conversational history, and supports quiz generation and evaluation workflows. " +
                              "ESBot follows a layered architecture with a React frontend, ASP.NET Core backend, " +
                              "PostgreSQL persistence, and optional integration with external or local large language models (LLMs). " +
                              "The API is designed to be modular, testable, and extensible, supporting both educational use cases and AI experimentation scenarios."
            });
        });
        
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