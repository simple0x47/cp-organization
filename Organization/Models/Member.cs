using Cuplan.Organization.Services;

namespace Cuplan.Organization.Models;

public class Member(string orgId, string email, IEnumerable<string> permissions, IEnumerable<string> roles)
{
    public string OrgId { get; set; } = orgId;
    public string Email { get; set; } = email;
    public IEnumerable<string> Permissions { get; set; } = permissions;
    public IEnumerable<string> Roles { get; set; } = roles;
}