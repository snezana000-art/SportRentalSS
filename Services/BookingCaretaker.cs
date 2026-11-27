using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

/// <summary>
/// BookingCaretaker - "Опекун" (Caretaker) в паттерне Хранитель.
/// Не знает, что внутри снимка: только сохраняет снимки и отдаёт их обратно.
/// </summary>
public class BookingCaretaker
{
    /// <summary>Сохраняет снимок брони. Вызывается ДО изменения брони.</summary>
    public void Save(AppDbContext db, Booking booking)
    {
        db.BookingSnapshots.Add(booking.CreateSnapshot());
    }

    /// <summary>Возвращает последний снимок брони или null, если снимков нет.</summary>
    public BookingSnapshot? GetLast(AppDbContext db, int bookingId)
    {
        return db.BookingSnapshots
            .Where(s => s.BookingId == bookingId)
            .OrderByDescending(s => s.Id)
            .FirstOrDefault();
    }

    /// <summary>Возвращает бронь в состояние из снимка и удаляет использованный снимок.</summary>
    public void Undo(AppDbContext db, Booking booking, BookingSnapshot snapshot)
    {
        booking.Restore(snapshot);
        db.BookingSnapshots.Remove(snapshot);
    }
}