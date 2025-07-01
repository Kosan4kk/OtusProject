using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Services;

public class BookingService
{
    private readonly IBookingRepository _repository;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly NotificationService _notificationService;

    public BookingService(
        IBookingRepository repository,
        NotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task<BookingResult> BookAsync(long userId, int roomId, DateTime start, DateTime end)
    {
        await _semaphore.WaitAsync();
        try
        {
            // Проверка доступности
            //bool isAvailable = await _repository.IsSlotAvailableAsync(roomId, start, end);

            //if (!isAvailable)
            //    return BookingResult.SlotNotAvailable;

            // Создание брони
            var booking = new StudioBooking
            {
                UserId = userId,
                RoomId = roomId,
                StartTime = start,
                EndTime = end,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(booking);

            // Уведомление админов
            //await _notificationService.NotifyAdmins($"Новая бронь: {start:g} - {end:g}");

            return BookingResult.Success;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public enum BookingResult { Success, SlotNotAvailable }
}