namespace Cuplan.Organization.Models;

public class Organization(string name, Address address, IEnumerable<string> permissions)
{
    public string Name { get; set; } = name;
    public Address Address { get; set; } = address;
    public IEnumerable<string> Permissions { get; set; } = permissions;
}