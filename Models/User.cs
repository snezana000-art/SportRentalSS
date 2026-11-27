namespace SportRental.Models;

/// <summary>Пользователь сайта: клиент или администратор.</summary>
public class User
{
    /// <summary>Номер пользователя в базе.</summary>
    public int Id { get; set; }

    /// <summary>Имя, которое видно на сайте.</summary>
    public string FullName { get; set; } = "";

    /// <summary>Email, он же логин. В базе не повторяется.</summary>
    public string Email { get; set; } = "";

    /// <summary>Хеш пароля (сам пароль нигде не хранится).</summary>
    public string PasswordHash { get; set; } = "";

    /// <summary>Роль: "Client" - обычный клиент, "Admin" - администратор.</summary>
    public string Role { get; set; } = "Client";
}