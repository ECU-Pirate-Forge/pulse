namespace Pulse.Services
{
    using Pulse.Models;

    /// <summary>
    /// Interface for question management service.
    /// This abstracts the backend API calls for creating, retrieving, and managing questions.
    /// </summary>
    public interface IQuestionService
    {
        /// <summary>
        /// Submits a question to the backend API
        /// </summary>
        /// <param name="question">The question model to submit</param>
        /// <returns>The created question with its ID assigned by the backend</returns>
        Task<QuestionModel> CreateQuestionAsync(QuestionModel question);

        /// <summary>
        /// Retrieves a single question by ID
        /// </summary>
        /// <param name="questionId">The ID of the question to retrieve</param>
        /// <returns>The question model or null if not found</returns>
        Task<QuestionModel?> GetQuestionAsync(string questionId);

        /// <summary>
        /// Retrieves all questions created by the current instructor
        /// </summary>
        /// <returns>List of question models</returns>
        Task<List<QuestionModel>> GetInstructorQuestionsAsync();

        /// <summary>
        /// Validates the MCQ options for a question
        /// </summary>
        /// <param name="options">The MCQ options to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> ValidateMCQOptionsAsync(List<string> options);
    }
}
