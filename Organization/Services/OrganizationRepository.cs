using Core;
using Cuplan.Organization.Models;
using MongoDB.Driver;
using Organization;
using Organization.Config;

namespace Cuplan.Organization.Services;

public class OrganizationRepository : IOrganizationRepository
{
    private const double DefaultTimeoutAfterSeconds = 15;
    private readonly IMongoCollection<IdentifiableOrganization> _collection;

    private readonly double _createTimeoutAfterSeconds;
    private readonly double _findByIdTimeoutAfterSeconds;

    private readonly ILogger<OrganizationRepository> _logger;

    public OrganizationRepository(ILogger<OrganizationRepository> logger, ConfigurationReader config,
        MongoClient client)
    {
        _logger = logger;
        _collection = client.GetDatabase(config.GetStringOrThrowException(ConfigurationReader.DatabaseKey))
            .GetCollection<IdentifiableOrganization>(
                config.GetStringOrThrowException("OrganizationRepository:Collection"));

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
            string message = $"failed to insert organization: {e}";
            _logger.LogInformation(message);
            return Result<string, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                message));
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

            bool hasNext = await cursor.MoveNextAsync().WaitAsync(TimeSpan.FromSeconds(_findByIdTimeoutAfterSeconds));

            if (!hasNext)
                return Result<Models.Organization, Error<ErrorKind>>.Err(
                    new Error<ErrorKind>(ErrorKind.NotFound, $"could not find org by id '{id}'"));

            IdentifiableOrganization? organization = cursor.Current.FirstOrDefault();

            if (organization is null)
                return Result<Models.Organization, Error<ErrorKind>>.Err(
                    new Error<ErrorKind>(ErrorKind.NotFound, $"could not find org by id '{id}'"));

            return Result<Models.Organization, Error<ErrorKind>>.Ok((Models.Organization)organization);
        }
        catch (TimeoutException)
        {
            string message = "timed out finding organization by id";
            _logger.LogInformation(message);
            return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.TimedOut,
                message));
        }
        catch (Exception e)
        {
            string message = $"failed to find organization by id: {e}";
            _logger.LogInformation(message);
            return Result<Models.Organization, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                message));
        }
    }
}