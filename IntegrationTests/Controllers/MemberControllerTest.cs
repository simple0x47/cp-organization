using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests.Controllers;

[Collection("Database")]
public class MemberControllerTest(WebApplicationFactory<Program> factory) : TestBase(factory)
{
    [Fact]
    public async Task CreateMember_WithNonExistingOrgId_Fails()
    {
        HttpClient client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);

        Member exampleMember =
            new("1234", "example@domain.com", Array.Empty<string>(), Array.Empty<string>());
        HttpResponseMessage response = await client.PostAsync("api/Member", JsonContent.Create(exampleMember));


        string failure = await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("org id not found", failure);
    }

    [Fact]
    public async Task CreateMember_WithExistingOrgId_Succeeds()
    {
        const int expectedMemberIdLength = 36;
        HttpClient client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);

        Models.Organization exampleOrg =
            new("a", new Address("a", "b", "c", "d", "e", "f", "g"),
                new[] { "a" });
        HttpResponseMessage orgResponse = await client.PostAsync("api/Organization", JsonContent.Create(exampleOrg));


        Assert.Equal(HttpStatusCode.OK, orgResponse.StatusCode);


        string orgId = await orgResponse.Content.ReadAsStringAsync();

        Member exampleMember =
            new(orgId, "example@domain.com", Array.Empty<string>(), Array.Empty<string>());
        HttpResponseMessage response = await client.PostAsync("api/Member", JsonContent.Create(exampleMember));


        string memberId = await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedMemberIdLength, memberId.Length);
    }
}