using Microsoft.AspNetCore.Http.HttpResults;

namespace Organization.Services;

public class OrganizationRepository : IOrganizationRepository
{
    public string? Create(Models.Organization organization)
    {
        return "Ok";
    }
}