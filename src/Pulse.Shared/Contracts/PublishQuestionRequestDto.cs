namespace Pulse.Shared.Contracts;

/// <summary>
/// Instructor request used to publish a question to all connected students.
/// </summary>
public sealed class PublishQuestionRequestDto
{
    public string SessionId { get; set; } = string.Empty;
    public string InstructorId { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "OpenEnded";
}
