using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot;
using WebApplication1.States;

public class BookingState : IUserState
{
    private readonly UserStateMachine _stateMachine;
    private readonly IServiceScopeFactory _scopeFactory; // Используем IServiceScopeFactory
    private DateTime? _startTime;
    private int? _hours;

    public BookingState(UserStateMachine stateMachine, IServiceScopeFactory scopeFactory)
    {
        _stateMachine = stateMachine;
        _scopeFactory = scopeFactory;
    }

    public async Task HandleMessageAsync(Message message, ITelegramBotClient botClient, CancellationToken ct)
    {
        if (!_startTime.HasValue)
        {
            // Пробуем несколько форматов даты
            string[] formats = {
                "yyyy-MM-dd HH:mm",
                "dd.MM.yyyy HH:mm",
                "dd/MM/yyyy HH:mm",
                "MM/dd/yyyy HH:mm"
            };

            if (DateTime.TryParseExact(
                message.Text,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var startTime))
            {
                _startTime = startTime;
                await botClient.SendMessage(
                    message.Chat.Id,
                    "⏳ Введите продолжительность в часах:",
                    cancellationToken: ct);
            }
            else
            {
                await botClient.SendMessage(
                    message.Chat.Id,
                    "❌ Неверный формат даты. Попробуйте еще раз (примеры: 2025-12-10 20:00, 10.12.2025 20:00):",
                    cancellationToken: ct);
            }
        }
        else if (!_hours.HasValue)
        {
            if (int.TryParse(message.Text, out int hours) && hours > 0)
            {
                _hours = hours;
                var endTime = _startTime.Value.AddHours(hours);

                using (var scope = _scopeFactory.CreateScope())
                {
                    // Создаем/получаем пользователя
                    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
                    var user = await userService.GetOrCreateUserAsync(
                        message.From.Id,
                        message.From.Username ?? "",
                        $"{message.From.FirstName} {message.From.LastName}",
                        message.Chat.Id
                    );

                    var bookingService = scope.ServiceProvider.GetRequiredService<BookingService>();
                    var bookingResult = await bookingService.BookAsync(
                        user.Id,
                        roomId: 1,
                        _startTime.Value,
                        endTime
                    );

                    // Обработка результатов
                    switch (bookingResult)
                    {
                        case BookingService.BookingResult.Success:
                            await botClient.SendMessage(
                                message.Chat.Id,
                                $"✅ Бронь создана!\n⏰ Время: {_startTime:g} - {endTime:t}\n⌛ Продолжительность: {hours} ч.",
                                cancellationToken: ct);
                            break;


                    }
                }

                _stateMachine.ClearState(message.From.Id);
            }
            else
            {
                await botClient.SendMessage(
                    message.Chat.Id,
                    "❌ Неверный формат. Введите целое число часов:",
                    cancellationToken: ct);
            }
        }
    }
}