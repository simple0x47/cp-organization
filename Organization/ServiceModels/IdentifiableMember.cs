using Cuplan.Organization.Models;

namespace Cuplan.Organization.ServiceModels;

public class IdentifiableMember(string id, Member member)
{
    public string Id { get; set; } = id;
    public string OrgId { get; set; } = member.OrgId;
    public string Email { get; set; } = member.Email;
    public IEnumerable<string> Permissions { get; set; } = member.Permissions;
    public IEnumerable<string> Roles { get; set; } = member.Roles;
}