using Microsoft.AspNetCore.SignalR;
using Pulse.Server.Hubs;
using Pulse.Shared.Contracts;

namespace Pulse.Server.Services;

public sealed class QuestionPublisher
{
    private readonly IHubContext<PulseQuestionHub> _hubContext;

    public QuestionPublisher(IHubContext<PulseQuestionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<NewQuestionPublishedDto> PublishAsync(
        PublishQuestionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var publishedQuestion = new NewQuestionPublishedDto
        {
            SessionId = request.SessionId,
            InstructorId = request.InstructorId,
            QuestionId = string.IsNullOrWhiteSpace(request.QuestionId)
                ? Guid.NewGuid().ToString("N")
                : request.QuestionId,
            QuestionText = request.QuestionText,
            QuestionType = request.QuestionType,
            PublishedAtUtc = DateTimeOffset.UtcNow
        };

        await _hubContext.Clients.All.SendAsync(
            PulseRealtimeConstants.NewQuestionPublishedEvent,
            publishedQuestion,
            cancellationToken);

        return publishedQuestion;
    }
}
