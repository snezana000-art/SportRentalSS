using SportRental.Models;

namespace SportRental.Services;

/// <summary>
/// «Фасад»: единая точка входа в логику сайта. Страницы работают только с этим классом
/// и ничего не знают о сервисах внутри него.
/// </summary>
public class RentalFacade
{
    private readonly AuthService _auth;
    private readonly InventoryService _inventory;
    private readonly BookingService _bookings;

    /// <summary>
    /// Срабатывает, когда пользователь вошёл или вышел .
    /// </summary>
    public event Action? AuthChanged;

    /// <summary>
    /// Создаёт фасад.
    /// </summary>
    /// <param name="auth">Сервис регистрации и входа.</param>
    /// <param name="inventory">Сервис каталога инвентаря.</param>
    /// <param name="bookings">Сервис бронирования.</param>
    public RentalFacade(AuthService auth, InventoryService inventory, BookingService bookings)
    {
        _auth = auth;
        _inventory = inventory;
        _bookings = bookings;

        _auth.Changed += () => AuthChanged?.Invoke();
    }

    public bool IsLoggedIn => _auth.IsLoggedIn;

    /// <summary>
    /// Является ли вошедший пользователь администратором.
    /// </summary>
    public bool IsAdmin => _auth.IsAdmin;

    /// <summary>
    /// Имя вошедшего пользователя .
    /// </summary>
    public string UserName => _auth.CurrentUser?.FullName ?? "";

    /// <summary>
    /// Регистрирует нового клиента и сразу выполняет вход.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <param name="email">Электронная почта.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Текст ошибки или нул, если всё хорошо.</returns>
    public Task<string?> RegisterAsync(string name, string email, string password)
    {
        return _auth.RegisterAsync(name, email, password);
    }

    /// <summary>
    /// Выполняет вход пользователя.
    /// </summary>
    /// <param name="email">Электронная почта.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Текст ошибки или null, если вход выполнен.</returns>
    public Task<string?> LoginAsync(string email, string password)
    {
        return _auth.LoginAsync(email, password);
    }

    /// <summary>
    /// Выполняет выход пользователя.
    /// </summary>
    public void Logout()
    {
        _auth.Logout();
    }

    public Task<List<Category>> GetCategoriesAsync()
    {
        return _inventory.GetCategoriesAsync();
    }

    /// <summary>
    /// Возвращает инвентарь, при необходимости только из одной категории.
    /// </summary>
    /// <param name="categoryId">Номер категории или нул для всего инвентаря.</param>
    
    public Task<List<Equipment>> GetEquipmentAsync(int? categoryId)
    {
        return _inventory.GetEquipmentAsync(categoryId);
    }


    /// <summary>
    /// Бронирует инвентарь для вошедшего пользователя.
    /// </summary>
    /// <param name="equipmentId">Номер инвентаря.</param>
    /// <param name="start">Дата начала аренды.</param>
    /// <param name="end">Дата окончания аренды.</param>
    /// <returns>Текст ошибки или null, если бронь создана.</returns>
    public async Task<string?> BookAsync(int equipmentId, DateTime start, DateTime end)
    {
        if (_auth.CurrentUser == null)
            return "Сначала войдите в аккаунт";

        return await _bookings.CreateAsync(_auth.CurrentUser.Id, equipmentId, start, end);
    }

    /// <summary>
    /// Возвращает брони вошедшего пользователя.
    /// </summary>
    /// <returns>Список броней .</returns>
public async Task<List<Booking>> GetMyBookingsAsync()
    {
        if (_auth.CurrentUser == null)
            return new List<Booking>();

        return await _bookings.GetUserBookingsAsync(_auth.CurrentUser.Id);
    }

    /// <summary>
    /// Отменяет бронь вошедшего пользователя.
    /// </summary>
    /// <param name="bookingId">Номер брони.</param>
    /// <returns>Текст ошибки или нул, если бронь отменена.</returns>
    public async Task<string?> CancelBookingAsync(int bookingId)
    {
        if (_auth.CurrentUser == null)
            return "Сначала войдите в аккаунт";

        return await _bookings.CancelAsync(bookingId, _auth.CurrentUser.Id);
    }

    /// <summary>
    /// Возвращает отменённую бронь в прежнее состояние.
    /// </summary>
    /// <param name="bookingId">Номер брони.</param>
    public async Task<string?> UndoCancelAsync(int bookingId)
    {
        if (_auth.CurrentUser == null)
            return "Сначала войдите в аккаунт";

        return await _bookings.UndoAsync(bookingId, _auth.CurrentUser.Id);
    }
}


