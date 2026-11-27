using Microsoft.AspNetCore.Identity;
using SportRental.Models;

namespace SportRental.Data;

/// <summary>Создаёт базу и заполняет её начальными данными при первом запуске.</summary>
public static class DbSeeder
{
    /// <summary>
    /// Создаёт базу, если её нет, и добавляет категории, инвентарь
    /// и администратора по умолчанию. Если данные уже есть, ничего не делает.
    /// </summary>
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Categories.Any())
        {
            return;
        }

        var bikes = new Category { Name = "Велосипеды" };
        var winter = new Category { Name = "Зимний спорт" };
        var wheels = new Category { Name = "Ролики и самокаты" };
        var tourism = new Category { Name = "Туризм" };
        db.Categories.AddRange(bikes, winter, wheels, tourism);

        db.Inventory.AddRange(
            new Equipment { Name = "Горный велосипед", Description = "21 скорость, амортизационная вилка, подходит для бездорожья.", PricePerDay = 600, Category = bikes },
            new Equipment { Name = "Городской велосипед", Description = "Лёгкий и удобный, для прогулок по городу.", PricePerDay = 450, Category = bikes },
            new Equipment { Name = "Беговые лыжи", Description = "Комплект: лыжи, палки и ботинки.", PricePerDay = 300, Category = winter },
            new Equipment { Name = "Горные лыжи", Description = "Комплект для склона: лыжи, палки, ботинки.", PricePerDay = 700, Category = winter },
            new Equipment { Name = "Сноуборд", Description = "Сноуборд с креплениями, на любой рост.", PricePerDay = 800, Category = winter },
            new Equipment { Name = "Роликовые коньки", Description = "Раздвижные, размеры 35-45, в комплекте защита.", PricePerDay = 350, Category = wheels },
            new Equipment { Name = "Самокат", Description = "Складной самокат с большими колёсами.", PricePerDay = 250, Category = wheels },
            new Equipment { Name = "Палатка 3-местная", Description = "Двухслойная, быстро собирается.", PricePerDay = 500, Category = tourism }
        );

        // администратор по умолчанию
        var admin = new User
        {
            FullName = "Администратор",
            Email = "admin@prokat.ru",
            Role = "Admin"
        };
        admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, "admin123");
        db.Users.Add(admin);

        db.SaveChanges();
    }
}