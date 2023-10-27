using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cuplan.Organization.IntegrationTests.Controllers;

[Collection("Database")]
public class RoleControllerTest : TestBase
{
    private const string RoleApi = "api/Role";

    private readonly HttpClient _client;

    public RoleControllerTest(WebApplicationFactory<Program> factory) : base(factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAccessToken);
    }

    [Fact]
    public async Task GetAdminRoleId_Succeeds()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RoleApi}/AdminRole");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Role? adminRole = await response.Content.ReadFromJsonAsync<Role>();

        Assert.NotNull(adminRole);
    }
}