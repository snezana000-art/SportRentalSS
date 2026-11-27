using Microsoft.EntityFrameworkCore;
using SportRental.Data;
using SportRental.Models;

namespace SportRental.Services;

// Работа с каталогом инвентаря
public class InventoryService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public InventoryService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    // categoryId = null - показать весь инвентарь
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

    // Проверка полей инвентаря. Возвращает текст ошибки или null.
    private string? Validate(string name, decimal price)
    {
        if (name.Trim() == "")
            return "Введите название";
        if (price <= 0)
            return "Цена должна быть больше нуля";
        return null;
    }

    public async Task<string?> AddAsync(string name, string description, decimal price, int categoryId)
    {
        var error = Validate(name, price);
        if (error != null)
            return error;

        using var db = _factory.CreateDbContext();

        if (!await db.Categories.AnyAsync(c => c.Id == categoryId))
            return "Выберите категорию";

        db.Inventory.Add(new Equipment
        {
            Name = name.Trim(),
            Description = description.Trim(),
            PricePerDay = price,
            CategoryId = categoryId
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(int id, string name, string description, decimal price, int categoryId)
    {
        var error = Validate(name, price);
        if (error != null)
            return error;

        using var db = _factory.CreateDbContext();

        var item = await db.Inventory.FindAsync(id);
        if (item == null)
            return "Инвентарь не найден";

        if (!await db.Categories.AnyAsync(c => c.Id == categoryId))
            return "Выберите категорию";

        item.Name = name.Trim();
        item.Description = description.Trim();
        item.PricePerDay = price;
        item.CategoryId = categoryId;

        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        using var db = _factory.CreateDbContext();

        if (await db.Bookings.AnyAsync(b => b.EquipmentId == id))
            return "Нельзя удалить: на этот инвентарь есть бронирования";

        var item = await db.Inventory.FindAsync(id);
        if (item == null)
            return "Инвентарь не найден";

        db.Inventory.Remove(item);
        await db.SaveChangesAsync();
        return null;
    }
}