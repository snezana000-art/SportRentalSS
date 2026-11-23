using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

/// <summary>
///  Сохраняет снимки броней и отдаёт их обратно,
/// но не знает, что внутри снимка.
/// </summary>
public class BookingCaretaker
{
    /// <summary>
    /// Сохраняет снимок брони. Вызывается ДО изменения брони.
    /// </summary>
    /// <param name="db">Подключение к базе данных, в котором идёт работа.</param>
    /// <param name="booking">Бронь, состояние которой нужно запомнить.</param>
    public void Save(AppDbContext db, Booking booking)
    {
        db.BookingSnapshots.Add(booking.CreateSnapshot());
    }

    /// <summary>
    /// Возвращает последний снимок брони.
    /// </summary>
    /// <param name="db">Подключение к базе данных.</param>
    /// <param name="bookingId">Номер брони.</param>
    /// <returns>Последний снимок или null, если снимков нет.</returns>
    public BookingSnapshot? GetLast(AppDbContext db, int bookingId)
    {
        return db.BookingSnapshots
            .Where(s => s.BookingId == bookingId)
            .OrderByDescending(s => s.Id)
            .FirstOrDefault();
    }

    /// <summary>
    /// Возвращает бронь в сохранённое состояние и удаляет использованный снимок.
    /// </summary>
    /// <param name="db">Подключение к базе данных.</param>
    /// <param name="booking">Бронь, которую нужно восстановить.</param>
    /// <param name="snapshot">Снимок, из которого восстанавливаем.</param>
    public void Undo(AppDbContext db, Booking booking, BookingSnapshot snapshot)
    {
        booking.Restore(snapshot);
        db.BookingSnapshots.Remove(snapshot);
    }
}