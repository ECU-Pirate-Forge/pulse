namespace Pulse.Services
{
    using Pulse.Models;

    /// <summary>
    /// Mock implementation of IQuestionService for development purposes.
    /// This allows Dev A to work independently without a backend until the API is ready.
    /// </summary>
    public class MockQuestionService : IQuestionService
    {
        private readonly List<QuestionModel> _questions = new();
        private readonly string _currentInstructorId = "instructor-001"; // Mock instructor ID

        /// <summary>
        /// Simulates creating a question by storing it in memory and assigning an ID
        /// </summary>
        public Task<QuestionModel> CreateQuestionAsync(QuestionModel question)
        {
            // Generate a mock ID
            question.Id = Guid.NewGuid().ToString();
            question.InstructorId = _currentInstructorId;
            question.CreatedAt = DateTime.UtcNow;

            _questions.Add(question);

            // Simulate network delay
            return Task.FromResult(question);
        }

        /// <summary>
        /// Retrieves a question from the mock storage
        /// </summary>
        public Task<QuestionModel?> GetQuestionAsync(string questionId)
        {
            var question = _questions.FirstOrDefault(q => q.Id == questionId);
            return Task.FromResult(question);
        }

        /// <summary>
        /// Retrieves all questions for the mock instructor
        /// </summary>
        public Task<List<QuestionModel>> GetInstructorQuestionsAsync()
        {
            var instructorQuestions = _questions
                .Where(q => q.InstructorId == _currentInstructorId)
                .ToList();

            return Task.FromResult(instructorQuestions);
        }

        /// <summary>
        /// Validates MCQ options (mock implementation)
        /// </summary>
        public Task<bool> ValidateMCQOptionsAsync(List<string> options)
        {
            // Mock validation logic
            bool isValid = options.Count >= 2 && // At least 2 options
                          options.Count <= 6 && // Max 6 options
                          options.All(o => !string.IsNullOrWhiteSpace(o)); // No empty options

            return Task.FromResult(isValid);
        }

        /// <summary>
        /// Gets all questions currently stored in memory (useful for debugging)
        /// </summary>
        public List<QuestionModel> GetAllMockQuestions() => new(_questions);

        /// <summary>
        /// Clears all mock data (useful for testing)
        /// </summary>
        public void ClearMockData() => _questions.Clear();
    }
}
