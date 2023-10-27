using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests.Controllers;

[Collection("Database")]
public class MemberControllerTest : TestBase
{
    private const string OrganizationApi = "api/Organization";
    private const string MemberApi = "api/Member";
    private const string DefaultTestUserId = "example@domain.com";

    private readonly HttpClient _client;

    public MemberControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);
    }

    [Fact]
    public async Task CreateMember_WithNonExistingOrgId_Fails()
    {
        Member exampleMember =
            new("1234", "example@domain.com", Array.Empty<string>(), Array.Empty<string>());
        HttpResponseMessage response = await _client.PostAsync(MemberApi, JsonContent.Create(exampleMember));


        string failure = await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("org id not found", failure);
    }

    [Fact]
    public async Task CreateMember_WithExistingOrgId_Succeeds()
    {
        const int expectedMemberIdLength = 36;

        string orgId = await CreateOrganization();


        string memberId = await CreateMember(orgId);


        Assert.Equal(expectedMemberIdLength, memberId.Length);
    }

    [Fact]
    public async Task UpdateMember_WithExistingUserId_Succeeds()
    {
        string orgId = await CreateOrganization();
        string memberId = await CreateMember(orgId);

        IEnumerable<string> permissions = new[] { "permission1", "permission2" };
        IEnumerable<string> roles = new[] { "role1", "role2" };
        Member member = new(orgId, DefaultTestUserId, permissions, roles);
        IdentifiableMember idMember = new(memberId, member);


        HttpResponseMessage updateResponse = await _client.PatchAsync(MemberApi, JsonContent.Create(idMember));


        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);


        HttpResponseMessage getResponse = await _client.GetAsync($"{MemberApi}/{memberId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        IdentifiableMember? getMember = await getResponse.Content.ReadFromJsonAsync<IdentifiableMember>();

        Assert.NotNull(getMember);
        Assert.True(getMember.Permissions.SequenceEqual(permissions));
        Assert.True(getMember.Roles.SequenceEqual(roles));
    }

    private async Task<string> CreateOrganization()
    {
        Models.Organization exampleOrg =
            new("a", new Address("a", "b", "c", "d", "e", "f", "g"),
                new[] { "a" });
        HttpResponseMessage orgResponse = await _client.PostAsync(OrganizationApi, JsonContent.Create(exampleOrg));


        Assert.Equal(HttpStatusCode.OK, orgResponse.StatusCode);


        return await orgResponse.Content.ReadAsStringAsync();
    }

    private async Task<string> CreateMember(string orgId)
    {
        Member exampleMember =
            new(orgId, DefaultTestUserId, Array.Empty<string>(), Array.Empty<string>());
        HttpResponseMessage response = await _client.PostAsync("api/Member", JsonContent.Create(exampleMember));


        string memberId = await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return memberId;
    }
}