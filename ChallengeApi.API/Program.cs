using ChallengeApi.API.Extensions;
using ChallengeApi.API.Middleware;
using ChallengeApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Middleware de excepciones — siempre primero
app.UseMiddleware<ExceptionMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Challenge API v1");
    options.RoutePrefix = string.Empty; // Swagger en la raíz
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ChallengeApi.Infrastructure.Persistence.Context.AppDbContext>();
    db.Database.Migrate();
}

app.Run();