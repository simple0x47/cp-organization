using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests.Controllers;

[Collection("Database")]
public class OrganizationControllerTest(WebApplicationFactory<Program> factory) : TestBase(factory)
{
    [Fact]
    public async Task CreateOrganization_ReturnsAnOrganizationId()
    {
        const int organizationIdLength = 24;
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);

        PartialOrganization examplePartialOrganization =
            new("a", new Address("a", "b", "c", "d", "e", "f", "g"),
                new[] { "a" });
        HttpResponseMessage response =
            await client.PostAsync("api/Organization", JsonContent.Create(examplePartialOrganization));


        string organizationId = await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(organizationIdLength, organizationId.Length);
    }
}