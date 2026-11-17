using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

/// <summary>
/// проверка занятости дат, создание брони, список броней клиента(бронь)
/// </summary>
public class BookingService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>
    /// сервис бронирования
    /// </summary>
    /// <param name="factory">фабрика для создания подключений к базе данных.</param>
    public BookingService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// проверяет, свободен ли инвентарь на выбранные даты.
    /// </summary>
    /// <param name="equipmentId">Номер инвентаря.</param>
    /// <param name="start">Дата начала аренды.</param>
    /// <param name="end">Дата окончания аренды.</param>
    /// <param name="ignoreBookingId">Бронь, которую не учитываем (0 - учитывать все).</param>
    /// <returns>тру, если инвентарь свободен на все эти даты.</returns>
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
    /// создаёт новую бронь после проверки дат
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
    /// возвращает все брони клиента, новые сверху
    /// </summary>
    /// <param name="userId">Номер клиента.</param>
    /// <returns>список броней вместе с названием инвентаря.</returns>
    public async Task<List<Booking>> GetUserBookingsAsync(int userId)
    {
        using var db = _factory.CreateDbContext();
        return await db.Bookings
            .Include(b => b.Equipment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }
}