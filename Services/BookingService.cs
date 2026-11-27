using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

// Вся логика бронирования: проверка дат, создание, отмена, возврат
public class BookingService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly BookingCaretaker _caretaker;

    public BookingService(IDbContextFactory<AppDbContext> factory, BookingCaretaker caretaker)
    {
        _factory = factory;
        _caretaker = caretaker;
    }

    // Свободен ли инвентарь на эти даты?
    // ignoreBookingId - бронь, которую не учитываем (нужно при восстановлении брони)
    public async Task<bool> IsAvailableAsync(int equipmentId, DateTime start, DateTime end, int ignoreBookingId = 0)
    {
        using var db = _factory.CreateDbContext();

        // занято, если есть активная бронь, у которой даты пересекаются с нашими
        bool busy = await db.Bookings.AnyAsync(b =>
            b.EquipmentId == equipmentId
            && b.Id != ignoreBookingId
            && (b.Status == BookingStatus.New || b.Status == BookingStatus.Confirmed)
            && start <= b.EndDate
            && end >= b.StartDate);

        return !busy;
    }

    // Создать бронь. Возвращает текст ошибки или null.
    public async Task<string?> CreateAsync(int userId, int equipmentId, DateTime start, DateTime end)
    {
        start = start.Date;
        end = end.Date;

        if (start < DateTime.Today)
            return "Нельзя забронировать на дату, которая уже прошла";
        if (end < start)
            return "Дата окончания не может быть раньше даты начала";

        using var db = _factory.CreateDbContext();

        var item = await db.Inventory.FindAsync(equipmentId);
        if (item == null)
            return "Инвентарь не найден";

        if (!await IsAvailableAsync(equipmentId, start, end))
            return "На эти даты инвентарь уже занят";

        int days = (end - start).Days + 1;

        db.Bookings.Add(new Booking
        {
            UserId = userId,
            EquipmentId = equipmentId,
            StartDate = start,
            EndDate = end,
            TotalPrice = item.PricePerDay * days,
            Status = BookingStatus.New
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(int userId)
    {
        using var db = _factory.CreateDbContext();
        return await db.Bookings
            .Include(b => b.Equipment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.Bookings
            .Include(b => b.Equipment)
            .Include(b => b.User)
            .OrderByDescending(b => b.Id)
            .ToListAsync();
    }

    // Отмена брони клиентом
    public async Task<string?> CancelAsync(int bookingId, int userId)
    {
        using var db = _factory.CreateDbContext();

        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
        if (booking == null)
            return "Бронь не найдена";
        if (booking.Status != BookingStatus.New && booking.Status != BookingStatus.Confirmed)
            return "Эту бронь уже нельзя отменить";

        _caretaker.Save(db, booking);          // сначала сохраняем снимок
        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync();
        return null;
    }

    // Вернуть отменённую бронь (Undo)
    public async Task<string?> UndoAsync(int bookingId, int userId)
    {
        using var db = _factory.CreateDbContext();

        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
        if (booking == null)
            return "Бронь не найдена";

        var snapshot = _caretaker.GetLast(db, bookingId);
        
if (snapshot == null)
            return "Нечего возвращать";

        // пока бронь была отменена, эти даты мог занять кто-то другой
        if (!await IsAvailableAsync(booking.EquipmentId, snapshot.StartDate, snapshot.EndDate, booking.Id))
            return "Нельзя вернуть бронь: эти даты уже заняты";

        _caretaker.Undo(db, booking, snapshot);
        await db.SaveChangesAsync();
        return null;
    }

    // Администратор подтверждает бронь
    public Task<string?> ConfirmAsync(int bookingId)
    {
        return ChangeStatusAsync(bookingId, BookingStatus.New, BookingStatus.Confirmed);
    }

    // Администратор отмечает возврат инвентаря
    public Task<string?> ReturnAsync(int bookingId)
    {
        return ChangeStatusAsync(bookingId, BookingStatus.Confirmed, BookingStatus.Returned);
    }

    private async Task<string?> ChangeStatusAsync(int bookingId, string from, string to)
    {
        using var db = _factory.CreateDbContext();

        var booking = await db.Bookings.FindAsync(bookingId);
        if (booking == null)
            return "Бронь не найдена";
        if (booking.Status != from)
            return "Статус брони уже изменился, обновите страницу";

        _caretaker.Save(db, booking);
        booking.Status = to;
        await db.SaveChangesAsync();
        return null;
    }
}