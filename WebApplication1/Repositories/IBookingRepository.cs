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
namespace WebApplication1.Repositories
{
    public interface IBookingRepository
    {
        Task AddAsync(StudioBooking booking);
        Task<StudioBooking> GetByIdAsync(int id);
        Task<List<StudioBooking>> GetByRoomAsync(int roomId, DateTime start, DateTime end);
    }
}
