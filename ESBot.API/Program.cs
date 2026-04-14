using System.Text.Json.Serialization;
using ESBot.API.Middleware;
using HealthChecks.UI.Client;
using ESBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace ESBot.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (connectionString == null)
            throw new NullReferenceException("connectionString is null! Please check  your configuration!");
        
        AddServices(builder, connectionString);
        
        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        AddHealthChecks(app);
        MigrateDatabase(app, logger);

        // =====================
        // Middleware
        // =====================
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        app.Run();
    }

    private static void MigrateDatabase(WebApplication app, ILogger<Program> logger)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EsBotDbContext>();
            var retries = 10;

            while (retries > 0)
            {
                try
                {
                    db.Database.Migrate();
                    break;
                }
                catch (Exception ex)
                {
                    retries--;
                    logger.LogWarning(ex, "Database not ready. Retrying... Attempts left: {Retries}",                        retries);
                    Thread.Sleep(3000);
                }
            }
            
            DbSeeder.Seed(db);
        }
    }
    
    private static void AddHealthChecks(WebApplication app)
    {
        app.MapHealthChecks("/health/live", new()
        {
            Predicate = _ => false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
    
    private static void AddServices(WebApplicationBuilder builder, String connectionString)
    {
        builder.Services.AddDbContext<EsBotDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddControllers();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<EsBotDbContext>("database")
            .AddNpgSql(connectionString, name: "postgres");
        
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
    }
}