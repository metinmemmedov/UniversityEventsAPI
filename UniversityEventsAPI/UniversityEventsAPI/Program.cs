var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                       // /openapi/v1.json
    app.UseSwaggerUI(options =>             // /swagger
    {
        options.SwaggerEndpoint("/openapi/v1.json", "UniversityEventsAPI v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
