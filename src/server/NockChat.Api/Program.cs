using NockChat.Api.Extensions;
using NockChat.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();

builder.Services.AddVersioning();
builder.Services.AddRateLimiting();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers();

await app.RunAsync();