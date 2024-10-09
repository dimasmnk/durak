using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Durak.Server.API.BackgroundServices;

public class BotBackgroundService(IConfiguration configuration, ILogger<BotBackgroundService> logger) : BackgroundService
{
    private readonly string _botToken = configuration.GetValue<string>("BOT_TOKEN")!;
    private readonly ILogger<BotBackgroundService> _logger = logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var botClient = new TelegramBotClient(_botToken);
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Bot started");

        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        switch (messageText.Split(' ').First())
        {
            case "/start":
                await botClient.SendTextMessageAsync(chatId, "Welcome to Durak Online! To play the game press Play button on the left side.", cancellationToken: cancellationToken);
                break;
            case "/help":
                await botClient.SendTextMessageAsync(chatId, "Type /start to start the game", cancellationToken: cancellationToken);
                break;
            default:
                break;
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
