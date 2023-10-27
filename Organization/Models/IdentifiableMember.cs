namespace Cuplan.Organization.Models;

public class IdentifiableMember
{
    public IdentifiableMember()
    {
    }

    public IdentifiableMember(string id, string orgId, string userId, IEnumerable<string> permissions,
        IEnumerable<string> roles)
    {
        Id = id;
        OrgId = orgId;
        UserId = userId;
        Permissions = permissions;
        Roles = roles;
    }


    public IdentifiableMember(string id, PartialMember partialMember)
    {
        Id = id;
        OrgId = partialMember.OrgId;
        UserId = partialMember.UserId;
        Permissions = partialMember.Permissions;
        Roles = partialMember.Roles;
    }

    public string Id { get; set; }
    public string OrgId { get; set; }
    public string UserId { get; set; }
    public IEnumerable<string> Permissions { get; set; }
    public IEnumerable<string> Roles { get; set; }
}