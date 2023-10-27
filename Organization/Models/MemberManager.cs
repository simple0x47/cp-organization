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
        Result<Organization, Error<ErrorKind>> findOrgResult = await orgRepository.FindById(member.OrgId);

        if (!findOrgResult.IsOk) return Result<string, Error<ErrorKind>>.Err(findOrgResult.UnwrapErr());

        Result<string, Error<ErrorKind>> createMemberResult = await memberRepository.Create(member);

        if (!createMemberResult.IsOk) return Result<string, Error<ErrorKind>>.Err(createMemberResult.UnwrapErr());

        string memberId = createMemberResult.Unwrap();

        return Result<string, Error<ErrorKind>>.Ok(memberId);
    }
}