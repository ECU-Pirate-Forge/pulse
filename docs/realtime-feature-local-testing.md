# Local Testing Guide: Live Question Realtime Pipeline

This guide covers how to test the "new question published" realtime flow locally.

## What You Are Testing

- Instructor publishes a question from the Blazor demo page.
- Server accepts `POST /api/instructor/questions/publish`.
- Server broadcasts `NewQuestionPublished` on SignalR hub `/hubs/questions`.
- Connected student clients receive and render the update instantly.

## Prerequisites

- .NET 9 SDK installed
- NuGet restore available
- Trusted local HTTPS certificate (recommended)

If needed:

```bash
dotnet dev-certs https --trust
```

## Project Paths

- Server project: `src/Pulse.Server/Pulse.Server.csproj`
- Client project: `src/Pulse.Client/Pulse.Client.csproj`
- Shared contract: `src/Pulse.Shared/Contracts`

## 1. Restore Dependencies

From repository root:

```bash
dotnet restore src/PulseRealtime.sln
```

## 2. Run Server and Client

Use two terminals from repo root.

Terminal A (Server):

```bash
dotnet run --project src/Pulse.Server/Pulse.Server.csproj --launch-profile https
```

Terminal B (Client):

```bash
dotnet run --project src/Pulse.Client/Pulse.Client.csproj --launch-profile https
```

Expected default URLs:

- Server: `https://localhost:7227` and `http://localhost:5075`
- Client: `https://localhost:7156` and `http://localhost:5089`

## 3. Verify Server Health

Open:

- `https://localhost:7227/`

Expected response:

```json
{"name":"Pulse Realtime Service","hub":"/hubs/questions","publishEndpoint":"/api/instructor/questions/publish"}
```

## 4. Open Live Demo Page

Open in browser:

- `https://localhost:7156/live-questions`

Open the same page in a second tab/window to simulate another connected student.

## 5. Publish a Question

Use these sample values:

- Session ID: `CSCI-401-SPR26`
- Instructor ID: `dr-nguyen`
- Question Type: `Multiple Choice` (dropdown)
- Question Text: `Which topic should we review next?`

Click **Publish Question**.

Expected result:

- Both tabs update instantly in **Incoming Updates**.
- Entry shows human-friendly metadata (session, instructor, question type, local time).

## 6. Validation Cases

### Required field validation

- Leave Question Text empty and click Publish.
- Expected: inline message `Question text is required.` and no new update.

### Connection failure handling

- Stop server while client page is open.
- Refresh `/live-questions`.
- Expected: page does not crash, status shows disconnected, warning message appears.

### Reconnect behavior

- Restart server and refresh the page.
- Expected: connection status returns to connected, publishing works again.

## 7. Optional API-level Publish Test (without UI form)

Run while server is up:

```bash
curl -X POST https://localhost:7227/api/instructor/questions/publish \
  -H "Content-Type: application/json" \
  -d '{"sessionId":"CSCI-401","instructorId":"inst1","questionId":"","questionText":"API publish test","questionType":"OpenEnded"}' \
  -k
```

Expected:

- HTTP 200 with `NewQuestionPublishedDto` JSON.
- Connected `/live-questions` clients receive the update.

## Troubleshooting

- `ERR_CONNECTION_REFUSED`:
  - Ensure server is running.
  - Ensure client `RealtimeServerBaseUrl` matches server protocol/port.
- HTTPS certificate issues:
  - Run `dotnet dev-certs https --trust`.
- SignalR negotiate fails:
  - Confirm server root endpoint responds on the same base URL used by client.

