using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

// запоминает кто был на сайте(вход,выход,регистрация)
public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser != null && CurrentUser.Role == "Admin";

    // срабатывает, когда кто-то вошёл или вышел (чтобы обновилось меню)
    public event Action? Changed;

    public AuthService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    // возвращает текст ошибки или нул
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
        user.PasswordHash = _hasher.HashPassword(user, password); 

        db.Users.Add(user);
        await db.SaveChangesAsync();

        CurrentUser = user;
        Changed?.Invoke();
        return null;
    }

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

        CurrentUser = user;
        Changed?.Invoke();
        return null;
    }

    public void Logout()
    {
        CurrentUser = null;
        Changed?.Invoke();
    }
}
