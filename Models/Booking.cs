namespace SportRental.Models;

// статус бронгирования
public static class BookingStatus
{
    public const string New = "Новая";
    public const string Confirmed = "Подтверждена";
    public const string Returned = "Возвращена";
    public const string Cancelled = "Отменена";
}

// Booking - "Создатель" (Originator) в паттерне Хранитель.
// сохраняет своё состояние в снимок и восстанавливается из него.
public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = BookingStatus.New;

    // делаем снимок текущего состояния
    public BookingSnapshot CreateSnapshot()
    {
        return new BookingSnapshot
        {
            BookingId = Id,
            StartDate = StartDate,
            EndDate = EndDate,
            TotalPrice = TotalPrice,
            Status = Status,
            SavedAt = DateTime.Now
        };
    }

    // возвращаем состояние из снимка
    public void Restore(BookingSnapshot snapshot)
    {
        StartDate = snapshot.StartDate;
        EndDate = snapshot.EndDate;
        TotalPrice = snapshot.TotalPrice;
        Status = snapshot.Status;
    }
}
