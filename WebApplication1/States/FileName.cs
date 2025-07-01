using System.Collections.Concurrent;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication1.Commands;

namespace WebApplication1.States
{
    public class UserStateMachine
    {
        // Используем статический словарь для сохранения состояний между запросами
        private static readonly ConcurrentDictionary<long, IUserState> _states = new();

        private readonly IServiceProvider _serviceProvider;

        public UserStateMachine(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetState(long userId, IUserState state)
            => _states[userId] = state;

        public void ClearState(long userId)
            => _states.TryRemove(userId, out _);

        public async Task ProcessMessageAsync(Message message, ITelegramBotClient botClient, CancellationToken ct)
        {
            var userId = message.From.Id;
            Console.WriteLine($"Processing message for user {userId}");

            if (_states.TryGetValue(userId, out var state))
            {
                Console.WriteLine($"User {userId} is in state: {state.GetType().Name}");
                await state.HandleMessageAsync(message, botClient, ct);
            }
            else
            {
                Console.WriteLine($"No state found for user {userId}, processing as command");
                using var scope = _serviceProvider.CreateScope();
                var commandProcessor = scope.ServiceProvider.GetRequiredService<CommandProcessor>();
                await commandProcessor.ProcessCommandAsync(message, ct);
            }
        }
    }
}