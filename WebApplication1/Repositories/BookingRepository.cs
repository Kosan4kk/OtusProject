using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WebApplication1.Repositories;
using Microsoft.Extensions.DependencyInjection;
namespace WebApplication1.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public BookingRepository(AppDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;

        }

        public async Task AddAsync(StudioBooking booking)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Bookings.AddAsync(booking);
            await context.SaveChangesAsync();
        }

        public async Task<StudioBooking> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<StudioBooking>> GetByRoomAsync(int roomId, DateTime start, DateTime end)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Participants)
                .Where(b => b.RoomId == roomId &&
                           b.StartTime >= start &&
                           b.EndTime <= end) 
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }
        public async Task<bool> IsSlotAvailableAsync(int roomId, DateTime start, DateTime end)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Bookings
                .Where(b => b.RoomId == roomId)
                .AllAsync(b => b.EndTime <= start || b.StartTime >= end);
        }
    }
}
