using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

/// <summary>
/// Логика бронирования: проверка занятости дат, создание, отмена и возврат брони.
/// </summary>
public class BookingService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly BookingCaretaker _caretaker;

    /// <summary>
    /// Создаёт сервис бронирования.
    /// </summary>
    /// <param name="factory">Фабрика для создания подключений к базе данных.</param>
    /// <param name="caretaker">Опекун, который хранит снимки броней.</param>
    public BookingService(IDbContextFactory<AppDbContext> factory, BookingCaretaker caretaker)
    {
        _factory = factory;
        _caretaker = caretaker;
    }

    /// <summary>
    /// Проверяет, свободен ли инвентарь на выбранные даты.
    /// </summary>
    /// <param name="equipmentId">Номер инвентаря.</param>
    /// <param name="start">Дата начала аренды.</param>
    /// <param name="end">Дата окончания аренды.</param>
    /// <param name="ignoreBookingId">Бронь, которую не учитываем (0 - учитывать все).</param>
    /// <returns>true, если инвентарь свободен на все эти даты.</returns>
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

    /// <summary>
    /// Создаёт новую бронь после проверки дат.
    /// </summary>
    /// <param name="userId">Номер клиента.</param>
    /// <param name="equipmentId">Номер инвентаря.</param>
    /// <param name="start">Дата начала аренды.</param>
    /// <param name="end">Дата окончания аренды.</param>
    /// <returns>Текст ошибки или null, если бронь создана.</returns>
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

    /// <summary>
    /// Возвращает все брони клиента, новые сверху.
    /// </summary>
    /// <param name="userId">Номер клиента.</param>
    /// <returns>Список броней вместе с названием инвентаря.</returns>
    public async Task<List<Booking>> GetUserBookingsAsync(int userId)
    {
        using var db = _factory.CreateDbContext();
        return await db.Bookings
            .Include(b => b.Equipment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }

    /// <summary>
    /// Отменяет бронь клиента. Перед отменой сохраняется снимок брони.
    /// </summary>
    /// <param name="bookingId">Номер брони.</param>

/// <param name="userId">Номер клиента (отменить можно только свою бронь).</param>
    /// <returns>Текст ошибки или null, если бронь отменена.</returns>
    public async Task<string?> CancelAsync(int bookingId, int userId)
    {
        using var db = _factory.CreateDbContext();

        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
        if (booking == null)
            return "Бронь не найдена";
        if (booking.Status != BookingStatus.New && booking.Status != BookingStatus.Confirmed)
            return "Эту бронь уже нельзя отменить";

        _caretaker.Save(db, booking);               // сначала сохраняем снимок
        booking.Status = BookingStatus.Cancelled;   // потом меняем бронь
        await db.SaveChangesAsync();
        return null;
    }

    /// <summary>
    /// Возвращает отменённую бронь в прежнее состояние (Undo).
    /// </summary>
    /// <param name="bookingId">Номер брони.</param>
    /// <param name="userId">Номер клиента.</param>
    /// <returns>Текст ошибки или null, если бронь возвращена.</returns>
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
}