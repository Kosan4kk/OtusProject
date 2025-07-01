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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication1.Commands
{
    public class CommandProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IBotCommand> _commands;

        public CommandProcessor(
            IServiceProvider serviceProvider,
            IEnumerable<IBotCommand> commands)
        {
            _serviceProvider = serviceProvider;
            _commands = commands;
        }

        public async Task ProcessCommandAsync(Message message, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(message.Text)) return;

            var commandText = message.Text.Split(' ').First();
            var command = _commands.FirstOrDefault(c => c.Command == commandText);

            if (command != null)
            {
                await command.ExecuteAsync(message,
                    _serviceProvider.GetRequiredService<ITelegramBotClient>(),
                    ct);
            }
            else
            {
                await _serviceProvider.GetRequiredService<ITelegramBotClient>()
                    .SendMessage(message.Chat.Id, "❌ Неизвестная команда", cancellationToken: ct);
            }
        }
    }
}