namespace Pulse.Shared.Contracts;

/// <summary>
/// Shared SignalR and API route names used by both server and client.
/// </summary>
public static class PulseRealtimeConstants
{
    public const string HubRoute = "/hubs/questions";
    public const string NewQuestionPublishedEvent = "NewQuestionPublished";
    public const string PublishQuestionApiRoute = "/api/instructor/questions/publish";
}
