using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Sdk;

namespace Cuplan.Organization.IntegrationTests.Controllers;

public class OrganizationControllerTest(WebApplicationFactory<Program> factory) : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateOrganization_ReturnsAnOrganizationId()
    {
        const int organizationIdLength = 36;
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);

        global::Cuplan.Organization.Models.Organization exampleOrganization =
            new ("a", new Address("a", "b", "c", "d", "e", "f", "g"),
                new[] { "a" });
        var response = await client.PostAsync("api/Organization", JsonContent.Create(exampleOrganization));

        
        string organizationId = await response.Content.ReadAsStringAsync();
        
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(organizationIdLength, organizationId.Length);
    }
}