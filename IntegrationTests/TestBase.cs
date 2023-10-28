using System.Diagnostics;
using System.Net.Http.Headers;
using Core.Secrets;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests;

public class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ISecretsManager _secretsManager;

    public TestBase(WebApplicationFactory<Program> factory)
    {
        ResolveDependencies();
        InitializeEnvironmentVariables();

        _secretsManager = new BitwardenSecretsManager(null);
        Factory = factory;
        ApiAccessToken = _secretsManager.get(Environment.GetEnvironmentVariable("API_ACCESS_TOKEN_SECRET"));

        Client = factory.CreateClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);
    }

    protected WebApplicationFactory<Program> Factory { get; set; }
    protected string? ApiAccessToken { get; }

    protected string ProjectRootPath { get; } =
        Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

    protected string SolutionRootPath { get; } =
        Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

    protected HttpClient Client { get; }

    private void ResolveDependencies()
    {
        string dependenciesPath = $"{SolutionRootPath}/compose";

        Process dependenciesScript = new();
        dependenciesScript.StartInfo.FileName = "bash";
        dependenciesScript.StartInfo.Arguments = $"{dependenciesPath}/deps.sh {dependenciesPath}";
        dependenciesScript.Start();
        dependenciesScript.WaitForExit();
    }

    private void InitializeEnvironmentVariables()
    {
        string? environmentMode = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environmentMode is null) environmentMode = "Development";

        string environmentFileDirectory = ProjectRootPath;
        string environmentFile = $"{environmentMode}.env";

        foreach (string line in File.ReadLines($"{environmentFileDirectory}/{environmentFile}"))
        {
            string[] keyValue = line.Split("=");

            if (keyValue.Length != 2) continue;

            Environment.SetEnvironmentVariable(keyValue[0], keyValue[1]);
        }
    }
}