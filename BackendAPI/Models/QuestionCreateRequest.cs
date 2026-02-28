namespace BackendAPI.Models;

public class QuestionCreateRequest
{
    public string QuestionText { get; set; } = string.Empty;
    public int QuestionTypeId { get; set; }
    public string ResponseCategory { get; set; } = string.Empty;
    public ResponseOptions ResponseOptions { get; set; } = new();
}
