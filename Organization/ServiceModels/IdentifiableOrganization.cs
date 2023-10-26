using Cuplan.Organization.Models;

namespace Cuplan.Organization.ServiceModels;

public class IdentifiableOrganization(string id, Models.Organization organization)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = organization.Name;
    public Address Address { get; set; } = organization.Address;
    public IEnumerable<string> Permissions { get; set; } = organization.Permissions;

    public static explicit operator Models.Organization(IdentifiableOrganization org) =>
        new (org.Name, org.Address, org.Permissions);
}