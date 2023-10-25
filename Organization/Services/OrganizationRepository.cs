using MongoDB.Driver;
using Organization.Models;

namespace Organization.Services;

public class OrganizationRepository(ILogger<OrganizationRepository> logger, MongoClient client) : IOrganizationRepository
{
    private const string Database = "cp_organization";
    private const string Collection = "organization";
    
    private IMongoCollection<IdentifiableOrganization> _collection = client.GetDatabase(Database).GetCollection<IdentifiableOrganization>(Collection);

    public async Task<string?> Create(Models.Organization organization)
    {
        string id = Guid.NewGuid().ToString();
        IdentifiableOrganization idOrg = new(id, organization);

        try
        {
            await _collection.InsertOneAsync(idOrg);
            return id;
        }
        catch (Exception e)
        {
            logger.LogInformation($"failed to insert organization: {e}");
            return null;
        }
    }
}