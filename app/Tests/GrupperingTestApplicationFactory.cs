using app.DAL;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace app.Tests;

public class GrupperingTestApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("DOTNET_CURRENT_MODE", "TESTING"); // Using this for certain services in Program.cs to make sure they aren't loading when testing.
        
        var postgresSqlContainer = new PostgreSqlBuilder("postgres:18-alpine")
            .WithDatabase("gruppering")
            .WithUsername("grouper")
            .WithPassword("Insecure")
            .WithExposedPort(5432)
            .Build();
        postgresSqlContainer.StartAsync().GetAwaiter().GetResult();
        
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            services.RemoveAll<DbContextOptions<GrupperingTypeContext>>();
            
            services.AddDbContextPool<GrupperingTypeContext>(options =>
            {
                options.UseNpgsql(postgresSqlContainer.GetConnectionString());
            });

        });

        
    }
}