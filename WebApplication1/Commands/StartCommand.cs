using WebApplication1.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WebApplication1.Commands;
namespace WebApplication1.Commands
{
    public class StartCommand : IBotCommand
    {
        public string Command => "/start";

        public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, CancellationToken ct)
        {
            await botClient.SendMessage(
                message.Chat.Id,
                "🎵 Добро пожаловать в музыкальную студию!\n" +
                "Доступные команды:\n" +
                "/book - забронировать студию\n" +
                "/schedule - посмотреть расписание",
                cancellationToken: ct);
        }
    }
}