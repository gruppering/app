using System.Net.Http.Headers;
using NUnit.Framework;

namespace app.Tests.Home;

public class HomeControllerTests
{
    private GrupperingTestApplicationFactory _factory;
    private HttpClient _client;

    
    [OneTimeSetUp] 
    public async Task Setup()
    {
        _factory = new GrupperingTestApplicationFactory();   
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
    }


    [Test]
    public async Task ExampleIntegrationTest()
    {
        var test = await _client.GetAsync($"/");
        var responseBody = await test.Content.ReadAsStringAsync();
        
        Assert.That(responseBody.IndexOf("Home Page - app"),Is.Not.EqualTo(-1));
    }
    
    
    [OneTimeTearDown]
    public async Task Completion()
    {
        using var scope = _factory.Services.CreateScope();
        // var exampleService = scope.ServiceProvider.GetService<ExampleService>();
        
    }
}