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
using WebApplication1.Services;
using WebApplication1.Services;
using System.Text;

namespace WebApplication1.Commands
{
    public class ParticipantsCommand : IBotCommand
    {
        private readonly ParticipantService _participantService;
        public string Command => "/participants";

        public ParticipantsCommand(ParticipantService participantService)
        {
            _participantService = participantService;
        }

        public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, CancellationToken ct)
        {
            var args = message.Text.Split(' ');
            if (args.Length < 2 || !int.TryParse(args[1], out int bookingId))
            {
                await botClient.SendMessage(
                    message.Chat.Id,
                    "Используйте: /participants [ID брони]",
                    cancellationToken: ct);
                return;
            }

            try
            {
                var participants = await _participantService.GetParticipantsAsync(bookingId);
                await botClient.SendMessage(
                    message.Chat.Id,
                    participants,
                    cancellationToken: ct);
            }
            catch (Exception ex)
            {
                await botClient.SendMessage(
                    message.Chat.Id,
                    $"Ошибка: {ex.Message}",
                    cancellationToken: ct);
            }
        }
        
    }
}