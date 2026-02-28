using MongoDB.Driver;
using BackendAPI.Data;
using BackendAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDb:ConnectionString")));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(
        builder.Configuration.GetValue<string>("MongoDb:DatabaseName")));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoDatabase>().GetCollection<Question>(
        builder.Configuration.GetValue<string>("MongoDb:QuestionsCollectionName")));
builder.Services.AddSingleton<IQuestionRepository, MongoQuestionRepository>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
