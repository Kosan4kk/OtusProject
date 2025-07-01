using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace WebApplication1.Services
{
    public interface ILessonObserver
    {
        Task NotifyAdmins( string message);
    }

    public class NotificationService : ILessonObserver
    {
        private readonly AppDbContext _context;
        private readonly ITelegramBotClient _botClient;

        public NotificationService(
            AppDbContext context,
            ITelegramBotClient botClient)
        {
            _context = context;
            _botClient = botClient;
        }

        public async Task NotifyAdmins(string message)
        {
            var admins = await _context.Users
                .Where(u => u.Role == "Admin")
                .ToListAsync();

            foreach (var admin in admins)
            {
                await _botClient.SendMessage(
                    admin.ChatId,
                    $"🔔 АДМИН: {message}");
            }
        }
    }
}
