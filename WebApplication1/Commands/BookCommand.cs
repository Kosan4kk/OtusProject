using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication1.States;

namespace WebApplication1.Commands
{
    public class BookCommand : IBotCommand
    {
        private readonly UserStateMachine _stateMachine;
        private readonly IServiceProvider _serviceProvider; // Добавляем поле
        public string Command => "/book";

        // Добавляем IServiceProvider в конструктор
        public BookCommand(
            UserStateMachine stateMachine,
            IServiceProvider serviceProvider)
        {
            _stateMachine = stateMachine;
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, CancellationToken ct)
        {
            // Создаем scope, чтобы получить BookingState
            using var scope = _serviceProvider.CreateScope();
            var bookingState = scope.ServiceProvider.GetRequiredService<BookingState>();

            _stateMachine.SetState(message.From.Id, bookingState);

            await botClient.SendMessage(
                message.Chat.Id,
                "🎸 Введите дату и время начала (формат: ГГГГ-ММ-ДД ЧЧ:ММ):",
                cancellationToken: ct);
        }
    }
}