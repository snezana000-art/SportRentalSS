namespace SportRental.Models;

// BookingSnapshot - "Хранитель" (Memento).
// будет хранить состояние брони на какой-то момент ( СНИМКИ_БРОНИ в базе )
public class BookingSnapshot
{
    public int Id { get; set; }
    public int BookingId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "";

    public DateTime SavedAt { get; set; }
}
