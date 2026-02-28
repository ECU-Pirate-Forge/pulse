using Pulse.Server.Hubs;
using Pulse.Server.Services;
using Pulse.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "PulseRealtime",
        policy =>
        {
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
        });
});
builder.Services.AddSignalR();
builder.Services.AddSingleton<QuestionPublisher>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("PulseRealtime");

app.MapGet(
    "/",
    () => Results.Ok(
        new
        {
            Name = "Pulse Realtime Service",
            Hub = PulseRealtimeConstants.HubRoute,
            PublishEndpoint = PulseRealtimeConstants.PublishQuestionApiRoute
        }));

app.MapPost(
    PulseRealtimeConstants.PublishQuestionApiRoute,
    async (
        PublishQuestionRequestDto request,
        QuestionPublisher questionPublisher,
        CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return Results.BadRequest(new { Error = "QuestionText is required." });
        }

        var publishedQuestion = await questionPublisher.PublishAsync(request, cancellationToken);
        return Results.Ok(publishedQuestion);
    });

app.MapHub<PulseQuestionHub>(PulseRealtimeConstants.HubRoute);

app.Run();
