using Core;
using Cuplan.Organization.Models;
using Organization;

namespace Cuplan.Organization.Services;

public interface IMemberRepository
{
    /// <summary>
    ///     Creates a membership between an email and an organization.
    /// </summary>
    /// <param name="member">The membership to be created.</param>
    /// <returns>Id of the created member, null if the creation failed.</returns>
    public Task<Result<string, Error<ErrorKind>>> Create(Member member);

    /// <summary>
    ///     Adds the specified role to the specified member.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="memberId"></param>
    /// <returns>Empty if the operation was a success, an error otherwise.</returns>
    public Task<Result<Empty, Error<ErrorKind>>> AddRoleToMember(string roleId, string memberId);
}