using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

/// <summary>
/// получение категорий и списка инвентаря.
/// </summary>
public class InventoryService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>
    /// создаёт сервис каталога.
    /// </summary>
    /// <param name="factory">фабрика для создания подключений к базе данных.</param>
    public InventoryService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// возвращает все категории, отсортированные по названию.
    /// </summary>
    /// <returns>список категорий.</returns>
    public async Task<List<Category>> GetCategoriesAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    /// <summary>
    /// возвращает инвентарь, при необходимости только из одной категории.
    /// </summary>
    /// <param name="categoryId">номер категории. Если нул, возвращается весь инвентарь.</param>
    /// <returns>список инвентаря вместе с названием категории.</returns>
    public async Task<List<Equipment>> GetEquipmentAsync(int? categoryId)
    {
        using var db = _factory.CreateDbContext();

        var query = db.Inventory.Include(e => e.Category).AsQueryable();
        if (categoryId != null)
        {
            query = query.Where(e => e.CategoryId == categoryId);
        }

        return await query.OrderBy(e => e.Name).ToListAsync();
    }
}