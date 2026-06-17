using ESBot.API;
using ESBot.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ESBot.Tests.api;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            // alte DB entfernen
            var descriptor = services
                .SingleOrDefault(d =>
                    d.ServiceType ==
                    typeof(DbContextOptions<EsBotDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Test DB einbauen
            services.AddDbContext<EsBotDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}