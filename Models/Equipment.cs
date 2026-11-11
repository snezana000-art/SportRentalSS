namespace SportRental.Models;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal PricePerDay { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
