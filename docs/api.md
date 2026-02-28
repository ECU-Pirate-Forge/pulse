# API Reference

Base path: /api

## POST /questions
Create a new question.

Request body:
- questionText (string, required)
- questionTypeId (int, required)
- responseCategory (string, required)
- responseOptions (object, required)
	- choices (string[], required, at least one)
	- answer (string, optional)

Responses:
- 201 Created with the created question
- 400 Bad Request when required fields are missing or choices are empty

## GET /questions/{id}
Fetch a question by id.

Responses:
- 200 OK with the question
- 404 Not Found when id does not exist