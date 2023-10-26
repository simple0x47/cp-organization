namespace Cuplan.Organization.Models;

public class IdentifiableOrganization(string id, Organization organization)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = organization.Name;
    public Address Address { get; set; } = organization.Address;
    public IEnumerable<string> Permissions { get; set; } = organization.Permissions;
}