namespace SportRental.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";

    // хранение хеша пароля
    public string PasswordHash { get; set; } = "";

    // "Client" - обычный клиент, "Admin" - администратор
    public string Role { get; set; } = "Client";
}
