using NockChat.Api.Exceptions;
using NockChat.Api.Extensions;
using NockChat.Application.Extensions;
using NockChat.Infrastructure.Extensions;
using NockChat.Infrastructure.Hubs;
using NockChat.Persistence.Extensions;

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
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

await app.RunAsync();