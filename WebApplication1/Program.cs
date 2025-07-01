using Microsoft.EntityFrameworkCore;
using WebApplication1.Commands;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.States;
using Telegram.Bot;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient);

// Репозитории
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Сервисы
builder.Services.AddSingleton<IServiceProvider>(provider => provider); 
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["BotToken"]!));
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<UserStateMachine>();
builder.Services.AddScoped<CommandProcessor>();
builder.Services.AddScoped<BookingState>();
builder.Services.AddScoped<UserService>(); 
// Команды
builder.Services.AddTransient<IBotCommand, StartCommand>();
builder.Services.AddTransient<IBotCommand, BookCommand>();
builder.Services.AddTransient<IBotCommand, ScheduleCommand>();
builder.Services.AddTransient<IBotCommand, ParticipantsCommand>();
builder.Services.AddScoped<BookingState>(provider =>
    new BookingState(
        provider.GetRequiredService<UserStateMachine>(),
        provider.GetRequiredService<IServiceScopeFactory>()

    ));
// Логирование
builder.Services.AddLogging();

// TelegramBotService
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<TelegramBotService>());

var app = builder.Build();

// Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();

    if (!db.Users.Any() && !db.StudioRooms.Any())
    {
        Console.WriteLine("Инициализация базы данных...");

        // Создаем администратора
        var admin = new User
        {
            Id = 1,
            TelegramId = 1, 
            Username = "admin",
            FullName = "Администратор",
            Role = "Admin",
            ChatId = 1 
        };
        db.Users.Add(admin);

        var room = new StudioRoom
        {
            Id = 1,
            Name = "Основная студия",
            Capacity = 5
        };
        db.StudioRooms.Add(room);

        await db.SaveChangesAsync();
        Console.WriteLine("База данных инициализирована");
    }
}

app.Run();