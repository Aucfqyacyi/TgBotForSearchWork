using MongoDB.Driver;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Repositories;

[SingletonService]
public class UserRepository
{
    private readonly MongoDbContext _mongoContext;

    public UserRepository(MongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    private FilterDefinition<User> GetFilterById(long chatId)
    {
        return Builders<User>.Filter.Eq(user => user.ChatId, chatId);
    }

    public User Get(long chatId, CancellationToken cancellationToken)
    {
        User? user = GetOrDefault(chatId, cancellationToken);
        if (user is null)
            throw new Exception($"UserSetting with chatId({chatId}) doesnot exist.");
        return user;
    }

    public User? GetOrDefault(long chatId, CancellationToken cancellationToken)
    {
        return _mongoContext.UserCollection.FindSync(GetFilterById(chatId), null, cancellationToken)
                                                   .FirstOrDefault(cancellationToken);
    }

    public List<User> GetAllActivated(int skip, int limit, CancellationToken cancellationToken)
    {
        var options = new FindOptions<User, User>()
        {
            Skip = skip,
            Limit = limit,
        };
        return _mongoContext.UserCollection.FindSync(user => user.IsActivated, options, cancellationToken)
                                                   .ToList();
    }

    public bool Create(long chatId, CancellationToken cancellationToken)
    {
        User? user = GetOrDefault(chatId, cancellationToken);
        if (user is null)
        {
            _mongoContext.UserCollection.InsertOne(new(chatId), null, cancellationToken);
            return true;
        }
        else
        {
            if (user.IsActivated is false)
            {
                Activate(chatId, true, cancellationToken);
                return true;
            }           
        }
        return false;
    }

    public void UpdateDescriptionLength(long chatId, int descriptionLength, CancellationToken cancellationToken)
    {
        descriptionLength = int.Min(int.Max(0, descriptionLength), 6000);
        var updateDefinition = Builders<User>.Update.Set(user => user.DescriptionLength, descriptionLength);
        _mongoContext.UserCollection.UpdateOne(GetFilterById(chatId), updateDefinition, null, cancellationToken);
    }

    public void Activate(long chatId, bool isActivated, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<User>.Update.Set(user => user.IsActivated, isActivated);
        _mongoContext.UserCollection.UpdateOne(GetFilterById(chatId), updateDefinition, null, cancellationToken);
    }
}
