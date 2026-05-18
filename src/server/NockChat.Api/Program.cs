using NockChat.Api.Exceptions;
using NockChat.Api.Extensions;
using NockChat.Application.Extensions;
using NockChat.Persistence.Extensions;
using NockChat.Infrastructure.Extensions;
using NockChat.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();

builder.Services.AddVersioning();
builder.Services.AddRateLimiting();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

await app.RunAsync();