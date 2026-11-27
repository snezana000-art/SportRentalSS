using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

/// <summary>
/// Регистрация, вход и выход. Помнит, кто сейчас вошёл на сайт,
/// и запоминает вход в браузере, чтобы он не пропадал после обновления страницы.
/// </summary>
public class AuthService
{
    // под этим именем в браузере хранится номер вошедшего пользователя
    private const string StorageKey = "userId";

    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly ProtectedLocalStorage _storage;
    private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

    /// <summary>Пользователь, который сейчас вошёл (или null, если никто не вошёл).</summary>
    public User? CurrentUser { get; private set; }

    /// <summary>Выполнен ли вход.</summary>
    public bool IsLoggedIn => CurrentUser != null;

    /// <summary>Вошёл ли администратор.</summary>
    public bool IsAdmin => CurrentUser != null && CurrentUser.Role == "Admin";

    /// <summary>Срабатывает, когда кто-то вошёл или вышел (чтобы обновилось меню).</summary>
    public event Action? Changed;

    public AuthService(IDbContextFactory<AppDbContext> factory, ProtectedLocalStorage storage)
    {
        _factory = factory;
        _storage = storage;
    }

    /// <summary>
    /// Регистрирует нового клиента и сразу выполняет вход.
    /// Возвращает текст ошибки или null, если всё хорошо.
    /// </summary>
    public async Task<string?> RegisterAsync(string name, string email, string password)
    {
        name = name.Trim();
        email = email.Trim().ToLower();

        if (name == "" || email == "" || password == "")
            return "Заполните все поля";
        if (!email.Contains("@"))
            return "Введите правильный email";
        if (password.Length < 6)
            return "Пароль должен быть не короче 6 символов";

        using var db = _factory.CreateDbContext();

        if (await db.Users.AnyAsync(u => u.Email == email))
            return "Пользователь с таким email уже есть";

        var user = new User { FullName = name, Email = email, Role = "Client" };
        user.PasswordHash = _hasher.HashPassword(user, password); // пароль хранится как хеш

        db.Users.Add(user);
        await db.SaveChangesAsync();

        await SignInAsync(user);
        return null;
    }

    /// <summary>
    /// Проверяет email и пароль и выполняет вход.
    /// Возвращает текст ошибки или null, если вход выполнен.
    /// </summary>
    public async Task<string?> LoginAsync(string email, string password)
    {
        email = email.Trim().ToLower();

        using var db = _factory.CreateDbContext();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return "Неверный email или пароль";

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return "Неверный email или пароль";

        await SignInAsync(user);
        return null;
    }

    /// <summary>Выход из аккаунта: забываем пользователя и стираем запись в браузере.</summary>
    public async Task LogoutAsync()
    {
        CurrentUser = null;

        try
        {
            await _storage.DeleteAsync(StorageKey);
        }
        catch
        {
            // если браузер недоступен, выход всё равно выполнен на сервере
        }

        Changed?.Invoke();
    }

    /// <summary>
    /// Восстанавливает вход после обновления страницы: берёт номер пользователя
    /// из браузера и достаёт пользователя из базы. Вызывается один раз при открытии сайта.
    /// </summary>
    public async Task RestoreAsync()
    {
        if (CurrentUser != null)
            return;

        try
        {
var saved = await _storage.GetAsync<int>(StorageKey);
            if (!saved.Success || saved.Value <= 0)
                return;

            using var db = _factory.CreateDbContext();
            var user = await db.Users.FindAsync(saved.Value);

            if (user != null)
            {
                CurrentUser = user;
                Changed?.Invoke();
            }
        }
        catch
        {
            // запись не удалось прочитать - считаем, что вход не выполнен
        }
    }

    // Запоминаем пользователя на сервере и в браузере
    private async Task SignInAsync(User user)
    {
        CurrentUser = user;
        await _storage.SetAsync(StorageKey, user.Id);
        Changed?.Invoke();
    }
}