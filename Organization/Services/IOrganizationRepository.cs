namespace Cuplan.Organization.Services;

public interface IOrganizationRepository
{ 
    /// <summary>
    /// Create an organization, returning its id if the operation has been completed successfully.
    /// </summary>
    /// <param name="organization">The organization to be created.</param>
    /// <returns>Id of the created organization, null if the creation failed.</returns>
    public Task<string?> Create(Models.Organization organization);
}