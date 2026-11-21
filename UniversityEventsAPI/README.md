# UniversityEventsAPI

1. Open UniversityEventsAPI.sln in Visual Studio 2026 with the .NET 10 SDK.
2. Press F5. Swagger UI opens at https://localhost:7100/swagger
3. In Postman: Import -> UniversityEventsAPI.postman_collection.json
   (variable baseUrl = https://localhost:7100; change the port if yours differs).

Extra query params on GET /api/events:
 search, location, upcomingOnly, minParticipants, sort=asc|desc
Export: GET /api/events/export
