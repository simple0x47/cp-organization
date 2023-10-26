using Core.Secrets;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests;

public class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    private ISecretsManager _secretsManager;
    
    public TestBase(WebApplicationFactory<Program> factory)
    {
        InitializeEnvironmentVariables();
        
        _secretsManager = new BitwardenSecretsManager(null);
        Factory = factory;
        ApiAccessToken = _secretsManager.get(Environment.GetEnvironmentVariable("API_ACCESS_TOKEN_SECRET"));
    }

    private void InitializeEnvironmentVariables()
    {
        string? environmentMode = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        if (environmentMode is null)
        {
            environmentMode = "Development";
        }

        string environmentFileDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string environmentFile = $"{environmentMode}.env";

        foreach (string line in File.ReadLines($"{environmentFileDirectory}/{environmentFile}"))
        {
            string[] keyValue = line.Split("=");

            if (keyValue.Length != 2)
            {
                continue;
            }
            
            Environment.SetEnvironmentVariable(keyValue[0], keyValue[1]);
        }   
    }
    
    protected WebApplicationFactory<Program> Factory { get; set; }
    protected string? ApiAccessToken { get; }
}