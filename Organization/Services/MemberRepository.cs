using Core;
using Cuplan.Organization.Models;
using Cuplan.Organization.ServiceModels;
using MongoDB.Driver;
using Organization;
using Organization.Config;

namespace Cuplan.Organization.Services;

public class MemberRepository : IMemberRepository
{
    private const double DefaultTimeoutAfterSeconds = 15;

    private readonly IMongoCollection<IdentifiableMember> _collection;

    private readonly double _createTimeoutAfterSeconds;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(ILogger<MemberRepository> logger, ConfigurationReader config, MongoClient client)
    {
        _logger = logger;
        _collection = client.GetDatabase(config.GetStringOrThrowException(ConfigurationReader.DatabaseKey))
            .GetCollection<IdentifiableMember>(config.GetStringOrThrowException("MemberRepository:Collection"));

        _createTimeoutAfterSeconds =
            config.GetDoubleOrDefault("MemberRepository:CreateTimeout", DefaultTimeoutAfterSeconds);
    }

    public async Task<Result<string, Error<ErrorKind>>> Create(Member member)
    {
        string id = Guid.NewGuid().ToString();
        IdentifiableMember idMember = new(id, member);

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

    public async Task<Result<Empty, Error<ErrorKind>>> AddRoleToMember(string roleId, string memberId)
    {
        throw new NotImplementedException();
    }
}