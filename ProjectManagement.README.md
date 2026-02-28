# Backend API - Student Survey API

ASP.NET Core Web API for real-time student survey question creation and persistence. Uses MongoDB and xUnit for unit tests.

## Projects
- BackendAPI: Web API
- BackendTests: xUnit tests

## Configuration
Update the MongoDB settings in BackendAPI/appsettings.json if needed:

MongoDb:
	ConnectionString = mongodb://localhost:27017
	DatabaseName = project_management
	QuestionsCollectionName = questions

## Run
- dotnet run --project BackendAPI/BackendAPI.csproj

## Test
- dotnet test Pulse.sln
