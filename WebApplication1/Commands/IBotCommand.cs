using WebApplication1.States;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace WebApplication1.Commands
{
    public interface IBotCommand
    {
        string Command { get; }
        Task ExecuteAsync(Message message, ITelegramBotClient botClient, CancellationToken ct);
    }
}
