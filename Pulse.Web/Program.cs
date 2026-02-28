using System.Text.Json;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/questions"));

app.MapGet("/questions", () =>
{
    var projectDir = Directory.GetCurrentDirectory();                 // .../pulse/Pulse.Web
    var repoRoot = Path.GetFullPath(Path.Combine(projectDir, ".."));  // .../pulse
    var jsonPath = Path.Combine(repoRoot, "MongoDB.json");

    if (!File.Exists(jsonPath))
        return Results.Text($"MongoDB.json not found at: {jsonPath}", "text/plain");

    var json = File.ReadAllText(jsonPath);
    using var doc = JsonDocument.Parse(json);

    if (doc.RootElement.ValueKind != JsonValueKind.Array)
    {
        var pretty = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        return Results.Content($"<pre>{WebUtility.HtmlEncode(pretty)}</pre>", "text/html");
    }

    var cards = new List<string>();
    int count = 0;

    foreach (var item in doc.RootElement.EnumerateArray())
    {
        if (item.ValueKind != JsonValueKind.Object) continue;
        count++;

        string question = GetString(item, "question") ?? GetString(item, "text") ?? "(no question)";
        string category = GetString(item, "responseCategory") ?? "(no category)";
        string typeId = GetString(item, "questionTypeId") ?? GetString(item, "questionType") ?? "unknown";

        // choices: responseOptions.choices (array)
        var choicesHtml = "";
        if (item.TryGetProperty("responseOptions", out var respOpts) &&
            respOpts.ValueKind == JsonValueKind.Object &&
            respOpts.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array)
        {
            var li = new List<string>();
            foreach (var c in choices.EnumerateArray())
            {
                li.Add($"<li>{WebUtility.HtmlEncode(c.ToString())}</li>");
            }

            choicesHtml = $@"
<div class='choices'>
  <div class='label'>Choices</div>
  <ul>{string.Join("", li)}</ul>
</div>";
        }

        // optional answer field
        var answer = GetString(item, "answer");
        var answerHtml = string.IsNullOrWhiteSpace(answer)
            ? ""
            : $@"<div class='answer'><span class='label'>Answer:</span> {WebUtility.HtmlEncode(answer)}</div>";

        cards.Add($@"
<div class='card'>
  <div class='top'>
    <div class='category'>{WebUtility.HtmlEncode(category)}</div>
    <div class='badge'>Type {WebUtility.HtmlEncode(typeId)}</div>
  </div>
  <div class='question'>{WebUtility.HtmlEncode(question)}</div>
  {choicesHtml}
  {answerHtml}
</div>");
    }

    var html = $@"<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8' />
  <title>Pulse - Questions</title>
  <style>
    body {{
      font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial;
      background: #0b0f17;
      color: #e8eefc;
      margin: 0;
    }}
    .wrap {{
      max-width: 900px;
      margin: 0 auto;
      padding: 28px 18px;
    }}
    .header {{
      display:flex;
      align-items:flex-end;
      justify-content:space-between;
      gap:12px;
      flex-wrap:wrap;
      margin-bottom: 16px;
    }}
    .muted {{ color:#a9b6d3; }}
    .pill {{
      display:inline-block;
      padding:6px 10px;
      border-radius:999px;
      background:#121a2a;
      border:1px solid #24324d;
      color:#a9b6d3;
      font-size: 13px;
    }}
    .grid {{
      display:grid;
      grid-template-columns: 1fr;
      gap: 12px;
    }}
    .card {{
      background:#121a2a;
      border:1px solid #24324d;
      border-radius: 14px;
      padding: 14px 16px;
    }}
    .top {{
      display:flex;
      justify-content:space-between;
      gap:10px;
      align-items:center;
      margin-bottom: 10px;
    }}
    .category {{
      color:#a9b6d3;
      font-size: 13px;
    }}
    .badge {{
      font-size: 12px;
      padding:4px 10px;
      border-radius:999px;
      background:#0a1222;
      border:1px solid #24324d;
      color:#a9b6d3;
      white-space:nowrap;
    }}
    .question {{
      font-size: 20px;
      line-height: 1.25;
      margin-bottom: 10px;
    }}
    .label {{
      color:#a9b6d3;
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: .06em;
    }}
    .choices ul {{
      margin: 8px 0 0 18px;
    }}
    .choices li {{
      margin: 4px 0;
    }}
    .answer {{
      margin-top: 10px;
      padding-top: 10px;
      border-top: 1px solid #24324d;
      color: #cfe0ff;
    }}
  </style>
</head>
<body>
  <div class='wrap'>
    <div class='header'>
      <div>
        <h1 style='margin:0'>Pulse Questions</h1>
        <div class='muted' style='margin-top:6px'>Sprint 1 demo — displaying questions from MongoDB.json</div>
      </div>
      <div class='pill'>Total: {count}</div>
    </div>

    <div class='grid'>
      {string.Join("", cards)}
    </div>
  </div>
</body>
</html>";

    return Results.Content(html, "text/html");
});

app.Run();

static string? GetString(JsonElement obj, string key)
{
    if (obj.TryGetProperty(key, out var val))
    {
        if (val.ValueKind == JsonValueKind.String) return val.GetString();
        return val.ToString();
    }
    return null;
}