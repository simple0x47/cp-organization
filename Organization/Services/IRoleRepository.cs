using Core;
using Cuplan.Organization.Models;
using Organization;

namespace Cuplan.Organization.Services;

public interface IRoleRepository
{
    /// <summary>
    ///     Gets the ID of the admin role.
    /// </summary>
    /// <returns>An <see cref="Role" /> or an error.</returns>
    public Task<Result<Role, Error<ErrorKind>>> GetAdminRole();
}