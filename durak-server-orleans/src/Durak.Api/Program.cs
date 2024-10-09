using Durak.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddPersistance();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddCustomCors(builder.Configuration);
builder.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddLogger();
builder.Services.AddSignalR();

var app = builder.Build();
app.ApplyMigrations();

app.UseCustomCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.MapSignalrEndpoints();

app.Run();
