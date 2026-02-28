using Microsoft.AspNetCore.Mvc;
using BackendAPI.Controllers;
using BackendAPI.Data;
using BackendAPI.Models;
using System.Linq;

namespace BackendTests;

public class QuestionsControllerTests
{
    private sealed class InMemoryQuestionRepository : IQuestionRepository
    {
        private readonly List<Question> _questions = new();

        public Task<Question> CreateAsync(Question question, CancellationToken cancellationToken)
        {
            question.Id ??= Guid.NewGuid().ToString("N");
            _questions.Add(question);
            return Task.FromResult(question);
        }

        public Task<Question?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_questions.FirstOrDefault(q => q.Id == id));
        }
    }

    [Fact]
    public async Task CreateQuestion_PersistsQuestionWithChoices()
    {
        var repository = new InMemoryQuestionRepository();
        var controller = new QuestionsController(repository);

        var request = new QuestionCreateRequest
        {
            QuestionText = "What is your favorite color?",
            QuestionTypeId = 1,
            ResponseCategory = "survey",
            ResponseOptions = new ResponseOptions
            {
                Choices = new List<string> { "Blue", "Green" }
            }
        };

        var result = await controller.CreateQuestion(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var question = Assert.IsType<Question>(created.Value);

        Assert.False(string.IsNullOrWhiteSpace(question.Id));
        Assert.Equal(2, question.ResponseOptions.Choices.Count);
    }

    [Fact]
    public async Task CreateQuestion_ReturnsBadRequest_WhenMissingRequiredFields()
    {
        var repository = new InMemoryQuestionRepository();
        var controller = new QuestionsController(repository);

        var request = new QuestionCreateRequest
        {
            QuestionText = "",
            QuestionTypeId = 1,
            ResponseCategory = "",
            ResponseOptions = new ResponseOptions { Choices = new List<string>() }
        };

        var result = await controller.CreateQuestion(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
