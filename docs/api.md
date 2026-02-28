# API Reference

## Real-time Question Pipeline

### SignalR Hub
- Route: `/hubs/questions`
- Event: `NewQuestionPublished`
- Event DTO: `Pulse.Shared.Contracts.NewQuestionPublishedDto`

```json
{
  "sessionId": "CSCI-401",
  "instructorId": "instructor-demo",
  "questionId": "f3f8e09b0a6f4538921384f14536140c",
  "questionText": "Which concept is still unclear after today's lecture?",
  "questionType": "OpenEnded",
  "publishedAtUtc": "2026-02-28T20:35:12.124+00:00"
}
```

### Instructor Publish Endpoint
- Method: `POST`
- Route: `/api/instructor/questions/publish`
- Request DTO: `Pulse.Shared.Contracts.PublishQuestionRequestDto`

```json
{
  "sessionId": "CSCI-401",
  "instructorId": "instructor-demo",
  "questionId": "",
  "questionText": "Which concept is still unclear after today's lecture?",
  "questionType": "OpenEnded"
}
```

### Shared Contract Source
- `src/Pulse.Shared/Contracts/PulseRealtimeConstants.cs`
- `src/Pulse.Shared/Contracts/NewQuestionPublishedDto.cs`
- `src/Pulse.Shared/Contracts/PublishQuestionRequestDto.cs`
