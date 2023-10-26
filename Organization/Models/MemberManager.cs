using Core;
using Cuplan.Organization.Services;
using Organization;

namespace Cuplan.Organization.Models;

public class MemberManager(IMemberRepository memberRepository, IOrganizationRepository orgRepository)
{
    /// <summary>
    ///     Creates the member.
    /// </summary>
    /// <returns>Id of the created member.</returns>
    public async Task<Result<string, Error<ErrorKind>>> Create(Member member)
    {
        Result<Organization, Error<ErrorKind>> org = await orgRepository.FindById(member.OrgId);

        if (!org.IsOk) return Result<string, Error<ErrorKind>>.Err(org.UnwrapErr());

        return Result<string, Error<ErrorKind>>.Ok("");
    }
}