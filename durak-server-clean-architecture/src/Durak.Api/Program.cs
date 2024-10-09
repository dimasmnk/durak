using Durak.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServicesFromAllAssemblies();
builder.Services.AddAllOptions(builder.Configuration);
builder.Services.AddProblemDetails();

var app = builder.Build();
app.MapAllEndpoints();
app.Run();