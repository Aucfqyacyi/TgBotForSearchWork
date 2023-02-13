using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UserRepository
{
    private readonly MongoDbContext _mongoContext;

    public UserRepository(MongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public User Add(long id, CancellationToken cancellationToken)
    {
        User? user = GetOrDefault(id, cancellationToken);
        if (user is null)
        {
            user = new User(id);
            _mongoContext.UserCollection.InsertOne(user, null, cancellationToken);
        }
        return user;
    }

    public void Remove(long id, CancellationToken cancellationToken)
    {
        _mongoContext.UserCollection.DeleteOne(user => user.Id == id, null, cancellationToken);
    }

    public void Replace(User user, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        _mongoContext.UserCollection.ReplaceOne(u => u.Id == user.Id, user, replaceOptions, cancellationToken);
    }

    public List<User> GetAll(CancellationToken cancellationToken)
    {
        return _mongoContext.UserCollection.FindSync(Builders<User>.Filter.Empty, null, cancellationToken)
                                     .ToList(cancellationToken);
    }

    public User? GetOrDefault(long id, CancellationToken cancellationToken)
    {
        return _mongoContext.UserCollection.FindSync(e => e.Id == id, null, cancellationToken)
                                     .FirstOrDefault(cancellationToken);
    }

    public User Get(long id, CancellationToken cancellationToken)
    {
        User? user = GetOrDefault(id, cancellationToken);
        if (user is null)
            throw new Exception($"User with id({id}) does not exist.");
        return user;
    }

    public bool AddUrlToVacancies(long userId, ObjectId urlId, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update.AddToSet(user => user.UrlIds, urlId);
        var result = _mongoContext.UserCollection.UpdateOne(user => user.Id == userId, update, null, cancellationToken);
        return result.ModifiedCount > 0;
    }

    public void RemoveUrlToVacancies(long userId, ObjectId urlId, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update.Pull(user => user.UrlIds, urlId);
        _mongoContext.UserCollection.UpdateOne(user => user.Id == userId, update, null, cancellationToken);
    }
}
