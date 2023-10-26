using Core;
using Cuplan.Organization.ServiceModels;
using MongoDB.Driver;
using Organization;
using Organization.Config;

namespace Cuplan.Organization.Services;

public class OrganizationRepository : IOrganizationRepository
{
    private const string Database = "cp_organization";
    private const string Collection = "organization";
    private const double DefaultTimeoutAfterSeconds = 15;

    private readonly IMongoCollection<IdentifiableOrganization> _collection;

    private readonly double _createTimeoutAfterSeconds;
    private readonly double _findByIdTimeoutAfterSeconds;

    private readonly ILogger<OrganizationRepository> _logger;

    public OrganizationRepository(ILogger<OrganizationRepository> logger, ConfigurationReader config,
        MongoClient client)
    {
        _logger = logger;
        _collection = client.GetDatabase(Database).GetCollection<IdentifiableOrganization>(Collection);

        _createTimeoutAfterSeconds =
            config.GetDoubleOrDefault("OrganizationRepository:CreateTimeout", DefaultTimeoutAfterSeconds);
        _findByIdTimeoutAfterSeconds =
            config.GetDoubleOrDefault("OrganizationRepository:FindByIdTimeout", DefaultTimeoutAfterSeconds);
    }

    public async Task<Result<string, Error<ErrorKind>>> Create(Models.Organization organization)
    {
        string id = Guid.NewGuid().ToString();
        IdentifiableOrganization idOrg = new(id, organization);

        try
        {
            await _collection.InsertOneAsync(idOrg).WaitAsync(TimeSpan.FromSeconds(_createTimeoutAfterSeconds));
            return Result<string, Error<ErrorKind>>.Ok(id);
        }
        catch (Exception e)
        {
            _logger.LogInformation($"failed to insert organization: {e}");
            return Result<string, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                $"failed to insert organization: {e}"));
        }
    }

    public async Task<Result<Models.Organization, Error<ErrorKind>>> FindById(string id)
    {
        try
        {
            IAsyncCursor<IdentifiableOrganization>? cursor =
                await _collection.FindAsync(p => p.Id.Equals(id))
                    .WaitAsync(TimeSpan.FromSeconds(_findByIdTimeoutAfterSeconds));

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
            _logger.LogInformation("timed out finding organization by id");
            return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                "timed out finding organization by id"));
        }
        catch (Exception e)
        {
            _logger.LogInformation($"failed to find organization by id: {e}");
            return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                $"failed to find organization by id: {e}"));
        }
    }
}