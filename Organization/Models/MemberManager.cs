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

    public async Task<Result<IdentifiableMember, Error<ErrorKind>>> Read(string memberId)
    {
        Result<IdentifiableMember, Error<ErrorKind>> findMemberResult = await memberRepository.FindById(memberId);

        if (!findMemberResult.IsOk)
            return Result<IdentifiableMember, Error<ErrorKind>>.Err(findMemberResult.UnwrapErr());

        IdentifiableMember idMember = findMemberResult.Unwrap();

        return Result<IdentifiableMember, Error<ErrorKind>>.Ok(idMember);
    }

    /// <summary>
    ///     Updates the member.
    /// </summary>
    /// <param name="idMember">The updated member.</param>
    /// <returns>An empty result indicating the operation was successful, or an error.</returns>
    public async Task<Result<Empty, Error<ErrorKind>>> Update(IdentifiableMember idMember)
    {
        Result<Empty, Error<ErrorKind>> updatePermissions =
            await memberRepository.SetPermissions(idMember.Id, idMember.Permissions);

        if (!updatePermissions.IsOk) return Result<Empty, Error<ErrorKind>>.Err(updatePermissions.UnwrapErr());

        Result<Empty, Error<ErrorKind>> updateRoles = await memberRepository.SetRoles(idMember.Id, idMember.Roles);

        if (!updateRoles.IsOk) return Result<Empty, Error<ErrorKind>>.Err(updateRoles.UnwrapErr());

        return Result<Empty, Error<ErrorKind>>.Ok(new Empty());
    }
}