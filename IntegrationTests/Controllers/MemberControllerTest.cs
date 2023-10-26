using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests.Controllers;

public class MemberControllerTest(WebApplicationFactory<Program> factory) : TestBase(factory)
{
    [Fact]
    public async Task CreateMember_WithNonExistingOrgId_Fails()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);

        Member exampleMember =
            new("1234", "example@domain.com", Array.Empty<string>(), Array.Empty<string>());
        var response = await client.PostAsync("api/Member", JsonContent.Create(exampleMember));

        
        string failure = await response.Content.ReadAsStringAsync();
        
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("org id not found", failure);
    }
}