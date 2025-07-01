using WebApplication1.Services;
using WebApplication1.States;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace WebApplication1.Commands
{
    public class ScheduleCommand : IBotCommand
    {
        private readonly ScheduleService _scheduleService;
        public string Command => "/schedule";

        public ScheduleCommand(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, CancellationToken ct)
        {
            var schedule = await _scheduleService.GetWeeklyScheduleAsync(1); // RoomId=1
            await botClient.SendMessage(
                message.Chat.Id,
                schedule,
                cancellationToken: ct);
        }
    }
}
