using AiAssistantApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AiService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Assistant API V1");
        c.RoutePrefix = ""; // This makes Swagger UI available at the root: http://localhost:5000/
    });
}

// Middleware
app.UseAuthorization();
app.MapControllers();
app.Run();
