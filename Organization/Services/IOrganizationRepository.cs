namespace Cuplan.Organization.Services;

public interface IOrganizationRepository
{ 
    /// <summary>
    /// Create an organization, returning its id if the operation has been completed successfully.
    /// </summary>
    /// <param name="organization">The organization to be created.</param>
    /// <returns>Id of the created organization, null if the creation failed.</returns>
    public Task<string?> Create(Models.Organization organization);

    /// <summary>
    /// Finds an organization by its id, returning the organization if it is found.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The organization whose id has been specified, null if it cannot be found or a failure occurs.</returns>
    public Task<Models.Organization?> FindById(string id);
}