using System.Text;
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
    public class ScheduleService
    {
        private readonly IBookingRepository _repository;

        public ScheduleService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GetWeeklyScheduleAsync(int roomId)
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-diff);
            var endOfWeek = startOfWeek.AddDays(7);

            var bookings = await _repository.GetByRoomAsync(
                roomId, startOfWeek, endOfWeek);


            var sb = new StringBuilder();
            sb.AppendLine($"📅 Расписание студии:");

            foreach (var booking in bookings)
            {
                sb.AppendLine($"\n⏱️ {booking.StartTime:g} - {booking.EndTime:t}");
                sb.AppendLine($"👤 Организатор: {booking.User.FullName}");
                sb.AppendLine($"👥 Участников: {booking.Participants.Count}");
                sb.AppendLine($"🆔 ID: {booking.Id}");
            }

            return sb.ToString();
        }
    }
}
