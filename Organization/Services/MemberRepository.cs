using Cuplan.Organization.Models;
using MongoDB.Driver;

namespace Cuplan.Organization.Services;

public class MemberRepository(ILogger<MemberRepository> logger, MongoClient client) : IMemberRepository
{
    public async Task<string?> Create(Member member)
    {
        return "";
    }
}