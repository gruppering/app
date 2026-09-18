using Microsoft.AspNetCore.Mvc.Testing;

namespace app.Tests;

public class GrupperingTestApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("DOTNET_CURRENT_MODE", "TESTING"); // Using this for certain services in Program.cs to make sure they aren't loading when testing.
        
        
        builder.ConfigureServices(services =>
        {
            
            
            // services.AddScoped<AircraftTypeService>();
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            // var typeContext = scope.ServiceProvider.GetRequiredService<TypeContext>();

            // Forsikrer seg at databasenm
            //typeContext.Database.EnsureCreated();


        });

        
    }
}