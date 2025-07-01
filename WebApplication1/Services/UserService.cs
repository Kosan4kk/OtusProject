using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Services;
public class UserService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UserService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<User> GetOrCreateUserAsync(long telegramId, string username, string fullName, long chatId)
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await context.Users.FindAsync(telegramId);
        if (user != null)
            return user;

        // Создаем нового пользователя
        user = new User
        {
            Id = telegramId,
            TelegramId = telegramId,
            Username = username,
            FullName = fullName,
            Role = "Client",
            ChatId = chatId
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        Console.WriteLine($"Создан новый пользователь: {user.Id} - {user.FullName}");

        return user;
    }
}