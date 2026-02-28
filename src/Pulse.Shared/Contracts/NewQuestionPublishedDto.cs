namespace Pulse.Shared.Contracts;

/// <summary>
/// Payload broadcast to students when an instructor publishes a new question.
/// </summary>
public sealed class NewQuestionPublishedDto
{
    public string SessionId { get; set; } = string.Empty;
    public string InstructorId { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public DateTimeOffset PublishedAtUtc { get; set; }
}
