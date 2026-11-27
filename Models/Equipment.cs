namespace SportRental.Models;

/// <summary>Единица инвентаря, которую можно взять напрокат.</summary>
public class Equipment
{
    /// <summary>Номер инвентаря в базе.</summary>
    public int Id { get; set; }

    /// <summary>Название, например "Горный велосипед".</summary>
    public string Name { get; set; } = "";

    /// <summary>Краткое описание для каталога.</summary>
    public string Description { get; set; } = "";

    /// <summary>Стоимость аренды за один день, в рублях.</summary>
    public decimal PricePerDay { get; set; }

    /// <summary>Номер категории, к которой относится инвентарь.</summary>
    public int CategoryId { get; set; }

    /// <summary>Категория инвентаря (подгружается вместе с ним).</summary>
    public Category? Category { get; set; }
}