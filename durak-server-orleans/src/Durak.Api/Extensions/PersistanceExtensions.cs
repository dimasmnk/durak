using Durak.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Durak.Api.Extensions;

public static class PersistanceExtensions
{
    public static void AddPersistance(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDurakDbContext, DurakDbContext>();
        builder.Services.AddDbContext<DurakDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetValue<string>("DATABASE_URL")!);
        });
    }

    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var dbContex = scope.ServiceProvider.GetRequiredService<DurakDbContext>();

            dbContex.Database.EnsureCreated();

            if (dbContex.Database.GetPendingMigrations().Any())
            {
                dbContex.Database.Migrate();
            }
        }
    }
}
