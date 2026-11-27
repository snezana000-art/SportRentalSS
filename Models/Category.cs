namespace SportRental.Models;

/// <summary>Категория инвентаря, например "Велосипеды" или "Зимний спорт".</summary>
public class Category
{
    /// <summary>Номер категории в базе.</summary>
    public int Id { get; set; }

    /// <summary>Название категории.</summary>
    public string Name { get; set; } = "";
}