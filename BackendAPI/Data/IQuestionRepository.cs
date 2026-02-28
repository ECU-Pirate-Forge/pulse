using BackendAPI.Models;

namespace BackendAPI.Data;

public interface IQuestionRepository
{
    Task<Question> CreateAsync(Question question, CancellationToken cancellationToken);
    Task<Question?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
