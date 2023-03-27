using AutoDIInjector.Attributes;
using MongoDB.Driver;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Repositories;

[SingletonService]
public class UserRepository
{
    private readonly IMongoDbContext _mongoContext;

    public UserRepository(IMongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    private FilterDefinition<User> GetFilterById(long chatId)
    {
        return Builders<User>.Filter.Eq(user => user.ChatId, chatId);
    }

    public async ValueTask<User> GetAsync(long chatId, CancellationToken cancellationToken)
    {
        User? user = await GetOrDefaultAsync(chatId, cancellationToken);
        return user ?? throw new Exception($"User with chatId({chatId}) doesnot exist.");
    }

    public async ValueTask<User?> GetOrDefaultAsync(long chatId, CancellationToken cancellationToken)
    {
        var cursor = await _mongoContext.UserCollection.FindAsync(GetFilterById(chatId), null, cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<List<User>> GetAllActivatedAsync(int skip, int limit, CancellationToken cancellationToken)
    {
        var options = new FindOptions<User, User>()
        {
            Skip = skip,
            Limit = limit,
        };
        var cursor = await _mongoContext.UserCollection.FindAsync(user => user.IsActivated, options, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async ValueTask<bool> CreateAsync(long chatId, CancellationToken cancellationToken)
    {
        User? user = await GetOrDefaultAsync(chatId, cancellationToken);
        if (user is null)
        {
            _mongoContext.UserCollection.InsertOne(new(chatId), null, cancellationToken);
            return true;
        }
        else
        {
            if (user.IsActivated is false)
            {
                await ActivateAsync(chatId, isActivated: true, cancellationToken);
                return true;
            }
        }
        return false;
    }

    public ValueTask UpdateAsync(long chatId, uint descriptionLength, CancellationToken cancellationToken)
    {
        descriptionLength = uint.Min(descriptionLength, 6000);
        var updateDefinition = Builders<User>.Update.Set(user => user.DescriptionLength, descriptionLength);
        return new(_mongoContext.UserCollection.UpdateOneAsync(GetFilterById(chatId), updateDefinition, null, cancellationToken));
    }

    public ValueTask ActivateAsync(long chatId, bool isActivated, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<User>.Update.Set(user => user.IsActivated, isActivated);
        return new(_mongoContext.UserCollection.UpdateOneAsync(GetFilterById(chatId), updateDefinition, null, cancellationToken));
    }
}
