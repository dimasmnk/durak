namespace Durak.Api.Extensions;

public static class LoggerExtensions
{
    public static void AddLogger(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "[dd/MM/yyyy hh:mm:ss] ";
            });
        });
    }
}
