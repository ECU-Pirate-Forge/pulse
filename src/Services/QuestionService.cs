namespace Pulse.Services
{
    using Pulse.Models;
    using System.Net.Http.Json;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Real implementation of IQuestionService that communicates with the backend API.
    /// 
    /// Note: This is currently a template. Uncomment and enable in Program.cs
    /// when the backend API is ready and tested.
    /// 
    /// Backend API Endpoints Expected:
    /// - POST /api/questions - Create a new question
    /// - GET /api/questions/{id} - Get a specific question
    /// - GET /api/questions - Get all questions for instructor
    /// - POST /api/questions/validate-mcq - Validate MCQ options
    /// </summary>
    public class QuestionService : IQuestionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuestionService> _logger;

        /// <summary>
        /// Base URL for the backend API (e.g., https://api.example.com)
        /// </summary>
        private const string API_BASE_URL = "/api";
        private const string QUESTIONS_ENDPOINT = $"{API_BASE_URL}/questions";
        private const string VALIDATE_MCQ_ENDPOINT = $"{API_BASE_URL}/questions/validate-mcq";

        public QuestionService(HttpClient httpClient, ILogger<QuestionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Creates a question by submitting it to the backend API
        /// </summary>
        public async Task<QuestionModel> CreateQuestionAsync(QuestionModel question)
        {
            try
            {
                _logger.LogInformation("Creating question: {QuestionText}", 
                    question.QuestionText[..Math.Min(50, question.QuestionText.Length)] + "...");

                var response = await _httpClient.PostAsJsonAsync(QUESTIONS_ENDPOINT, question);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create question: {StatusCode} {ErrorContent}", 
                        response.StatusCode, errorContent);
                    
                    throw new HttpRequestException(
                        $"Failed to create question. Status: {response.StatusCode}. " +
                        $"Response: {errorContent}");
                }

                var createdQuestion = await response.Content.ReadFromJsonAsync<QuestionModel>();
                _logger.LogInformation("Question created successfully with ID: {QuestionId}", createdQuestion?.Id);

                return createdQuestion ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while creating question");
                throw new InvalidOperationException("Failed to communicate with the backend API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating question");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific question by ID from the backend
        /// </summary>
        public async Task<QuestionModel?> GetQuestionAsync(string questionId)
        {
            try
            {
                _logger.LogInformation("Retrieving question with ID: {QuestionId}", questionId);

                var response = await _httpClient.GetAsync($"{QUESTIONS_ENDPOINT}/{questionId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Question not found: {QuestionId}", questionId);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var question = await response.Content.ReadFromJsonAsync<QuestionModel>();

                return question;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while retrieving question {QuestionId}", questionId);
                throw new InvalidOperationException(
                    $"Failed to retrieve question {questionId}", ex);
            }
        }

        /// <summary>
        /// Retrieves all questions created by the current instructor
        /// </summary>
        public async Task<List<QuestionModel>> GetInstructorQuestionsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all questions for instructor");

                var response = await _httpClient.GetAsync(QUESTIONS_ENDPOINT);
                response.EnsureSuccessStatusCode();

                var questions = await response.Content.ReadFromJsonAsync<List<QuestionModel>>();

                _logger.LogInformation("Retrieved {Count} questions for instructor", questions?.Count ?? 0);

                return questions ?? new List<QuestionModel>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while retrieving instructor questions");
                throw new InvalidOperationException(
                    "Failed to retrieve your questions", ex);
            }
        }

        /// <summary>
        /// Validates MCQ options by submitting them to the backend
        /// </summary>
        public async Task<bool> ValidateMCQOptionsAsync(List<string> options)
        {
            try
            {
                if (!options.Any())
                {
                    _logger.LogWarning("Empty options list provided for validation");
                    return false;
                }

                _logger.LogInformation("Validating MCQ options: {OptionCount} options", options.Count);

                var payload = new { options };
                var response = await _httpClient.PostAsJsonAsync(VALIDATE_MCQ_ENDPOINT, payload);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("MCQ validation failed with status: {StatusCode}", response.StatusCode);
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                return result != null && result.ContainsKey("isValid") && (bool)result["isValid"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating MCQ options");
                // Don't throw - let client-side validation catch this
                return false;
            }
        }

        /// <summary>
        /// Updates an existing question (placeholder for future implementation)
        /// </summary>
        public async Task<QuestionModel> UpdateQuestionAsync(string questionId, QuestionModel question)
        {
            try
            {
                _logger.LogInformation("Updating question: {QuestionId}", questionId);

                var response = await _httpClient.PutAsJsonAsync(
                    $"{QUESTIONS_ENDPOINT}/{questionId}", question);

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<QuestionModel>();
                return result ?? throw new InvalidOperationException("Failed to deserialize question response");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while updating question");
                throw new InvalidOperationException("Failed to update question", ex);
            }
        }

        /// <summary>
        /// Deletes a question (placeholder for future implementation)
        /// </summary>
        public async Task DeleteQuestionAsync(string questionId)
        {
            try
            {
                _logger.LogInformation("Deleting question: {QuestionId}", questionId);

                var response = await _httpClient.DeleteAsync($"{QUESTIONS_ENDPOINT}/{questionId}");
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Question deleted successfully: {QuestionId}", questionId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while deleting question");
                throw new InvalidOperationException("Failed to delete question", ex);
            }
        }
    }
}
