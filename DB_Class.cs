public class Question
{
    public string Id { get; set; }
    public string QuestionText { get; set; }
    public int QuestionTypeId { get; set; }
    public string ResponseCategory { get; set; }
    public ResponseOptions ResponseOptions { get; set; }
}

public class ResponseOptions
{
    public List<string> Choices { get; set; }
    public string? Answer { get; set; } // null if survey type
}