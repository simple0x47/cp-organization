using Core;
using Cuplan.Organization.Services;
using Organization;

namespace Cuplan.Organization.Models;

public class MemberManager(IMemberRepository memberRepository, IOrganizationRepository orgRepository)
{
    /// <summary>
    /// Creates the member.
    /// </summary>
    /// <returns>Id of the created member.</returns>
    public async Task<Result<string, Error<ErrorKind>>> Create(Member member)
    {
        Organization? org = await orgRepository.FindById(member.OrgId);

        if (org is null)
        {
            return Result<string, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.OrganizationNotFound,
                "org id not found"));
        }

        return Result<string, Error<ErrorKind>>.Ok("");
    }
}