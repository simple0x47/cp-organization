using Core;
using Cuplan.Organization.Models;
using MongoDB.Driver;
using Organization;
using Organization.Config;

namespace Cuplan.Organization.Services;

public class MemberRepository : IMemberRepository
{
    private const double DefaultTimeoutAfterSeconds = 15;

    private readonly IMongoCollection<IdentifiableMember> _collection;

    private readonly double _createTimeoutAfterSeconds;
    private readonly double _findByIdTimeoutAfterSeconds;
    private readonly ILogger<MemberRepository> _logger;
    private readonly double _setPermissionsTimeoutAfterSeconds;
    private readonly double _setRolesTimeoutAfterSeconds;

    public MemberRepository(ILogger<MemberRepository> logger, ConfigurationReader config, MongoClient client)
    {
        _logger = logger;
        _collection = client.GetDatabase(config.GetStringOrThrowException(ConfigurationReader.DatabaseKey))
            .GetCollection<IdentifiableMember>(config.GetStringOrThrowException("MemberRepository:Collection"));

        _createTimeoutAfterSeconds =
            config.GetDoubleOrDefault("MemberRepository:CreateTimeout", DefaultTimeoutAfterSeconds);
        _findByIdTimeoutAfterSeconds =
            config.GetDoubleOrDefault("MemberRepository:FindByIdTimeout", DefaultTimeoutAfterSeconds);
        _setPermissionsTimeoutAfterSeconds =
            config.GetDoubleOrDefault("MemberRepository:SetPermissionsTimeout", DefaultTimeoutAfterSeconds);
        _setRolesTimeoutAfterSeconds =
            config.GetDoubleOrDefault("MemberRepository:SetRolesTimeout", DefaultTimeoutAfterSeconds);
    }

    public async Task<Result<Empty, Error<ErrorKind>>> SetPermissions(string memberId, IEnumerable<string> permissions)
    {
        try
        {
            FilterDefinition<IdentifiableMember>? filter = Builders<IdentifiableMember>.Filter.Eq(m => m.Id, memberId);
            UpdateDefinition<IdentifiableMember>? update =
                Builders<IdentifiableMember>.Update.Set(m => m.Permissions, permissions);

            UpdateResult result = await _collection.UpdateOneAsync(filter, update)
                .WaitAsync(TimeSpan.FromSeconds(_setPermissionsTimeoutAfterSeconds));

            if (result.ModifiedCount != 1)
                return Result<Empty, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.NotFound,
                    $"member with id '{memberId}' not found"));

            return Result<Empty, Error<ErrorKind>>.Ok(new Empty());
        }
        catch (TimeoutException)
        {
            string message = "timed out setting permissions";
            _logger.LogInformation(message);
            return Result<Empty, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.TimedOut, message));
        }
        catch (Exception e)
        {
            string message = $"failed to set permissions: {e}";
            _logger.LogInformation(message);
            return Result<Empty, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError, message));
        }
    }

    public async Task<Result<Empty, Error<ErrorKind>>> SetRoles(string memberId, IEnumerable<string> roles)
    {
        try
        {
            FilterDefinition<IdentifiableMember>? filter = Builders<IdentifiableMember>.Filter.Eq(m => m.Id, memberId);
            UpdateDefinition<IdentifiableMember>? update =
                Builders<IdentifiableMember>.Update.Set(m => m.Roles, roles);

            UpdateResult result = await _collection.UpdateOneAsync(filter, update)
                .WaitAsync(TimeSpan.FromSeconds(_setRolesTimeoutAfterSeconds));

            if (result.ModifiedCount != 1)
                return Result<Empty, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.NotFound,
                    $"member with id '{memberId}' not found"));

            return Result<Empty, Error<ErrorKind>>.Ok(new Empty());
        }
        catch (TimeoutException)
        {
            string message = "timed out setting roles";
            _logger.LogInformation(message);
            return Result<Empty, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.TimedOut, message));
        }
        catch (Exception e)
        {
            string message = $"failed to set roles: {e}";
            _logger.LogInformation(message);
            return Result<Empty, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError, message));
        }
    }

    public async Task<Result<string, Error<ErrorKind>>> Create(PartialMember partialMember)
    {
        string id = Guid.NewGuid().ToString();
        IdentifiableMember idMember = new(id, partialMember);

        try
        {
            await _collection.InsertOneAsync(idMember).WaitAsync(TimeSpan.FromSeconds(_createTimeoutAfterSeconds));
            return Result<string, Error<ErrorKind>>.Ok(id);
        }
        catch (TimeoutException)
        {
            string message = "timed out creating member";
            _logger.LogInformation(message);
            return Result<string, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.TimedOut, message));
        }
        catch (Exception e)
        {
            string message = $"failed to create member: {e}";
            _logger.LogInformation(message);
            return Result<string, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError, message));
        }
    }

    public async Task<Result<IdentifiableMember, Error<ErrorKind>>> FindById(string memberId)
    {
        try
        {
            IAsyncCursor<IdentifiableMember>? cursor =
                await _collection.FindAsync(p => p.Id.Equals(memberId))
                    .WaitAsync(TimeSpan.FromSeconds(_findByIdTimeoutAfterSeconds));

            if (cursor is null)
                return Result<IdentifiableMember, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                    "find async cursor is null"));

            bool hasNext = await cursor.MoveNextAsync().WaitAsync(TimeSpan.FromSeconds(_findByIdTimeoutAfterSeconds));

            if (!hasNext)
                return Result<IdentifiableMember, Error<ErrorKind>>.Err(
                    new Error<ErrorKind>(ErrorKind.NotFound, $"could not find member by id '{memberId}'"));

            IdentifiableMember? idMember = cursor.Current.FirstOrDefault();

            if (idMember is null)
                return Result<IdentifiableMember, Error<ErrorKind>>.Err(
                    new Error<ErrorKind>(ErrorKind.NotFound, $"could not find member by id '{memberId}'"));

            return Result<IdentifiableMember, Error<ErrorKind>>.Ok(idMember);
        }
        catch (TimeoutException)
        {
            string message = "timed out finding member by id";
            _logger.LogInformation(message);
            return Result<IdentifiableMember, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.TimedOut,
                message));
        }
        catch (Exception e)
        {
            string message = $"failed to find member by id: {e}";
            _logger.LogInformation(message);
            return Result<IdentifiableMember, Error<ErrorKind>>.Err(new Error<ErrorKind>(ErrorKind.StorageError,
                message));
        }
    }
}