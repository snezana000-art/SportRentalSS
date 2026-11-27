using SportRental.Models;

namespace SportRental.Services;

// RentalFacade - "Фасад" (Facade).
// Страницы сайта обращаются только к нему и не знают про остальные сервисы.
public class RentalFacade
{
    private readonly AuthService _auth;
    private readonly InventoryService _inventory;
    private readonly BookingService _bookings;

    public event Action? AuthChanged;

    public RentalFacade(AuthService auth, InventoryService inventory, BookingService bookings)
    {
        _auth = auth;
        _inventory = inventory;
        _bookings = bookings;

        _auth.Changed += () => AuthChanged?.Invoke();
    }

    public bool IsLoggedIn => _auth.IsLoggedIn;
    public bool IsAdmin => _auth.IsAdmin;
    public string UserName => _auth.CurrentUser?.FullName ?? "";

    public Task<string?> RegisterAsync(string name, string email, string password)
        => _auth.RegisterAsync(name, email, password);

    public Task<string?> LoginAsync(string email, string password)
        => _auth.LoginAsync(email, password);

    public void Logout() => _auth.Logout();

    public Task<List<Category>> GetCategoriesAsync()
        => _inventory.GetCategoriesAsync();

    public Task<List<Equipment>> GetEquipmentAsync(int? categoryId)
        => _inventory.GetEquipmentAsync(categoryId);

    public async Task<string?> BookAsync(int equipmentId, DateTime start, DateTime end)
    {
        if (_auth.CurrentUser == null)
            return "Сначала войдите в аккаунт";

        return await _bookings.CreateAsync(_auth.CurrentUser.Id, equipmentId, start, end);
    }

    public async Task<List<Booking>> GetMyBookingsAsync()
    {
        if (_auth.CurrentUser == null)
            return new List<Booking>();

        return await _bookings.GetUserBookingsAsync(_auth.CurrentUser.Id);
    }

    public async Task<string?> CancelBookingAsync(int bookingId)
    {
        if (_auth.CurrentUser == null)
            return "Сначала войдите в аккаунт";

        return await _bookings.CancelAsync(bookingId, _auth.CurrentUser.Id);
    }

    public async Task<string?> UndoCancelAsync(int bookingId)
    {
        if (_auth.CurrentUser == null)
            return "Сначала войдите в аккаунт";

        return await _bookings.UndoAsync(bookingId, _auth.CurrentUser.Id);
    }

    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        if (!IsAdmin)
            return new List<Booking>();

        return await _bookings.GetAllAsync();
    }

    public async Task<string?> ConfirmBookingAsync(int bookingId)
    {
        if (!IsAdmin)
            return "Нет доступа";

        return await _bookings.ConfirmAsync(bookingId);
    }

    public async Task<string?> ReturnBookingAsync(int bookingId)
    {
        if (!IsAdmin)
            return "Нет доступа";

        return await _bookings.ReturnAsync(bookingId);
    }

    public async Task<string?> AddEquipmentAsync(string name, string description, decimal price, int categoryId)
    {
        if (!IsAdmin)
            return "Нет доступа";

        return await _inventory.AddAsync(name, description, price, categoryId);
    }

    public async Task<string?> UpdateEquipmentAsync(int id, string name, string description, decimal price, int categoryId)
    {
        if (!IsAdmin)
            return "Нет доступа";

        return await _inventory.UpdateAsync(id, name, description, price, categoryId);
    }

    public async Task<string?> DeleteEquipmentAsync(int id)
    {
        if (!IsAdmin)
            return "Нет доступа";

        return await _inventory.DeleteAsync(id);
    }
}