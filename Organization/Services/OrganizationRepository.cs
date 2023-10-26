using Core;
using Cuplan.Organization.ServiceModels;
using MongoDB.Driver;
using Organization;

namespace Cuplan.Organization.Services;

public class OrganizationRepository
    (ILogger<OrganizationRepository> logger, MongoClient client) : IOrganizationRepository
{
    private const string Database = "cp_organization";
    private const string Collection = "organization";

    private readonly IMongoCollection<IdentifiableOrganization> _collection =
        client.GetDatabase(Database).GetCollection<IdentifiableOrganization>(Collection);

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

    public async Task<Result<Models.Organization, Error<ErrorKind>>> FindById(string id)
    {
        try
        {
            IAsyncCursor<IdentifiableOrganization>? cursor =
                await _collection.FindAsync(p => p.Id.Equals(id)).WaitAsync(TimeSpan.FromSeconds(15));

            if (cursor is null)
                return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                    "find async cursor is null"));

            if (!await cursor.AnyAsync())
                return Result<Models.Organization, Error<ErrorKind>>.Err(
                    new Error<ErrorKind>(ErrorKind.OrganizationNotFound, $"could not find org by id '{id}'"));

            IdentifiableOrganization organization = cursor.First();

            return Result<Models.Organization, Error<ErrorKind>>.Ok((Models.Organization)organization);
        }
        catch (TimeoutException)
        {
            logger.LogInformation("timed out finding organization by id");
            return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                "timed out finding organization by id"));
        }
        catch (Exception e)
        {
            logger.LogInformation($"failed to find organization by id: {e}");
            return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                $"failed to find organization by id: {e}"));
        }
    }
}