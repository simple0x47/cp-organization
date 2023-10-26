using Cuplan.Organization.Models;

namespace Cuplan.Organization.Services;

public interface IMemberRepository
{
    /// <summary>
    /// Creates a membership between an email and an organization.
    /// </summary>
    /// <param name="member">The membership to be created.</param>
    /// <returns>Id of the created member, null if the creation failed.</returns>
    public Task<string?> Create(Member member);
}