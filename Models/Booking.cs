namespace SportRental.Models;

/// <summary>Возможные статусы брони.</summary>
public static class BookingStatus
{
    public const string New = "Новая";
    public const string Confirmed = "Подтверждена";
    public const string Returned = "Возвращена";
    public const string Cancelled = "Отменена";
}

/// <summary>
/// Booking - "Создатель" (Originator) в паттерне Хранитель.
/// Умеет сохранять своё состояние в снимок и восстанавливаться из него.
/// </summary>
public class Booking
{
    /// <summary>Номер брони в базе.</summary>
    public int Id { get; set; }

    /// <summary>Номер клиента, который сделал бронь.</summary>
    public int UserId { get; set; }

    /// <summary>Клиент.</summary>
    public User? User { get; set; }

    /// <summary>Номер забронированного инвентаря.</summary>
    public int EquipmentId { get; set; }

    /// <summary>Забронированный инвентарь.</summary>
    public Equipment? Equipment { get; set; }

    /// <summary>Первый день аренды.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Последний день аренды.</summary>
    public DateTime EndDate { get; set; }

    /// <summary>Общая стоимость аренды.</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Текущий статус (см. BookingStatus).</summary>
    public string Status { get; set; } = BookingStatus.New;

    /// <summary>Делает снимок текущего состояния брони.</summary>
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

    /// <summary>Возвращает бронь в состояние, сохранённое в снимке.</summary>
    public void Restore(BookingSnapshot snapshot)
    {
        StartDate = snapshot.StartDate;
        EndDate = snapshot.EndDate;
        TotalPrice = snapshot.TotalPrice;
        Status = snapshot.Status;
    }
}