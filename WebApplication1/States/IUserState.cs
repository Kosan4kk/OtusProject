using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace WebApplication1.States
{
    public interface IUserState
    {
        Task HandleMessageAsync(Message message, ITelegramBotClient botClient, CancellationToken ct);
    }
}
