using MongoDB.Driver;
using BackendAPI.Models;

namespace BackendAPI.Data;

public class MongoQuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _collection;

    public MongoQuestionRepository(IMongoCollection<Question> collection)
    {
        _collection = collection;
    }

    public async Task<Question> CreateAsync(Question question, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(question, cancellationToken: cancellationToken);
        return question;
    }

    public async Task<Question?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _collection.Find(q => q.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
