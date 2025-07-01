using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Text;

namespace WebApplication1.Services
{
    public class ParticipantService
    {
        private readonly AppDbContext _context;
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();

        public ParticipantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddParticipantAsync(int bookingId, long userId, string fullName)
        {
            var semaphore = _locks.GetOrAdd(bookingId, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Participants)
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null) return;

                if (booking.Participants.Count >= booking.Room.Capacity)
                    throw new Exception("Достигнут лимит участников");

                if (!booking.Participants.Any(p => p.UserId == userId))
                {
                    booking.Participants.Add(new Participant
                    {
                        UserId = userId,
                        FullName = fullName
                    });
                    await _context.SaveChangesAsync();
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<string> GetParticipantsAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Participants)
                .Include(b => b.Room)
                .FirstAsync(b => b.Id == bookingId);

            var sb = new StringBuilder();
            sb.AppendLine($"👥 Участники брони #{bookingId}:");

            foreach (var p in booking.Participants)
            {
                sb.AppendLine($"- {p.FullName}");
            }

            sb.AppendLine($"\nВсего: {booking.Participants.Count}/{booking.Room.Capacity}");
            return sb.ToString();
        }
    }
}