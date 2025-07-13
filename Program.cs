using AiAssistantApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AiService>();

var app = builder.Build();

// ✅ Enable Swagger always (not just in Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Assistant API V1");
    c.RoutePrefix = ""; // This makes Swagger UI available at root: /
});

// Middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
