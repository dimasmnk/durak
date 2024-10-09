using Durak.Core.Common.Options;

namespace Durak.Api.Extensions;

public static class OptionExtensions
{
    public static void AddAllOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var botOptions = new BotOptions
        {
            BotToken = configuration.GetValue<string>("BOT_TOKEN") ?? throw new NullReferenceException(nameof(BotOptions.BotToken))
        };

        services.AddSingleton(botOptions);

        var authOptions = new AuthOptions
        {
            JwtSecretKey = configuration.GetValue<string>("SECRET_KEY") ?? throw new NullReferenceException(nameof(AuthOptions.JwtSecretKey))
        };

        services.AddSingleton(authOptions);
    }
}