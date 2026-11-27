namespace SportRental.Models;

/// <summary>
/// BookingSnapshot - "Хранитель" (Memento).
/// Хранит состояние брони на какой-то момент. Это таблица снимков брони в базе.
/// </summary>
public class BookingSnapshot
{
    /// <summary>Номер снимка в базе.</summary>
    public int Id { get; set; }

    /// <summary>Номер брони, с которой сделан снимок.</summary>
    public int BookingId { get; set; }

    /// <summary>Дата начала аренды на момент снимка.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Дата окончания аренды на момент снимка.</summary>
    public DateTime EndDate { get; set; }

    /// <summary>Стоимость на момент снимка.</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Статус брони на момент снимка.</summary>
    public string Status { get; set; } = "";

    /// <summary>Когда снимок был сделан.</summary>
    public DateTime SavedAt { get; set; }
}