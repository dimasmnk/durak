using Durak.Server.API.BackgroundServices;
using Durak.Server.API.Context;
using Durak.Server.API.Endpoints;
using Durak.Server.API.Hubs;
using Durak.Server.API.Repositories;
using Durak.Server.API.Services;
using Durak.Server.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "cors", corsBuilder =>
    corsBuilder.WithOrigins(builder.Configuration.GetValue<string>("ORIGIN")!)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});

builder.Services.AddDbContext<DurakDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetValue<string>("DATABASE")!);
});

builder.Services.AddHostedService<BotBackgroundService>();

builder.Services.AddScoped<IDurakDbContext, DurakDbContext>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IRoomTokenService, RoomTokenService>();
builder.Services.AddScoped<IRoomListService, RoomListService>();
builder.Services.AddScoped<IRoomListEventService, RoomListEventService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomEventService, RoomEventService>();
builder.Services.AddScoped<IRoomInfoService, RoomInfoService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton<ISignalrConnectionService, SignalrConnectionService>();
builder.Services.AddSignalR();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("SECRET_KEY")!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
{
    var dbContex = scope.ServiceProvider.GetRequiredService<DurakDbContext>();

    if (dbContex.Database.GetPendingMigrations().Any())
    {
        dbContex.Database.Migrate();

    }
}

app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationEndpoints();
app.MapPlayerEndpoints();
app.MapRoomEndpoints();
app.MapHub<RoomListHub>("/room-list");
app.MapHub<RoomHub>("/room");

app.Run();