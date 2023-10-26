using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Organization.Models;
using Xunit;

namespace Cuplan.Organization.IntegrationTests;

public class OrganizationControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrganizationControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateOrganization_ReturnsAnOrganizationId()
    {
        const int organizationIdLength = 36;
        var client = _factory.CreateClient();

        global::Organization.Models.Organization exampleOrganization =
            new global::Organization.Models.Organization("a", new Address("a", "b", "c", "d", "e", "f", "g"),
                new[] { "a" });
        var response = await client.PostAsync("api/Organization", JsonContent.Create(exampleOrganization));

        
        string organizationId = await response.Content.ReadAsStringAsync();
        
        
        Assert.Equal(organizationIdLength, organizationId.Length);
    }
}