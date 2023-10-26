using Cuplan.Organization.Services;

namespace Cuplan.Organization.Models;

public class MemberManager(IMemberRepository memberRepository, IOrganizationRepository orgRepository)
{
    /// <summary>
    /// Creates the member.
    /// </summary>
    /// <returns>Id of the created member.</returns>
    public async Task<string?> Create(Member member)
    {
        Organization? org = await orgRepository.FindById(member.OrgId);

        if (org is null)
        {
            return null;
        }

        return "";
    }
}