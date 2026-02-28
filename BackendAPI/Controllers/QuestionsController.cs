using Microsoft.AspNetCore.Mvc;
using BackendAPI.Data;
using BackendAPI.Models;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _repository;

    public QuestionsController(IQuestionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion([FromBody] QuestionCreateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.QuestionText) ||
            string.IsNullOrWhiteSpace(request.ResponseCategory))
        {
            return BadRequest("QuestionText and ResponseCategory are required.");
        }

        if (request.ResponseOptions is null || request.ResponseOptions.Choices.Count == 0)
        {
            return BadRequest("At least one choice is required.");
        }

        var question = new Question
        {
            QuestionText = request.QuestionText.Trim(),
            QuestionTypeId = request.QuestionTypeId,
            ResponseCategory = request.ResponseCategory.Trim(),
            ResponseOptions = new ResponseOptions
            {
                Answer = request.ResponseOptions.Answer?.Trim(),
                Choices = request.ResponseOptions.Choices
                    .Where(choice => !string.IsNullOrWhiteSpace(choice))
                    .Select(choice => choice.Trim())
                    .ToList()
            }
        };

        if (question.ResponseOptions.Choices.Count == 0)
        {
            return BadRequest("At least one non-empty choice is required.");
        }

        await _repository.CreateAsync(question, cancellationToken);

        return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestionById(string id, CancellationToken cancellationToken)
    {
        var question = await _repository.GetByIdAsync(id, cancellationToken);

        if (question is null)
        {
            return NotFound();
        }

        return Ok(question);
    }
}
