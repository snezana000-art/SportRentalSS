using Microsoft.EntityFrameworkCore;
using SportRental.Models;

namespace SportRental.Data;

/// <summary>Контекст базы данных: через него сайт читает и записывает таблицы.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Таблица пользователей.</summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>Таблица категорий.</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>Таблица инвентаря.</summary>
    public DbSet<Equipment> Inventory => Set<Equipment>();

    /// <summary>Таблица бронирований.</summary>
    public DbSet<Booking> Bookings => Set<Booking>();

    /// <summary>Таблица снимков броней (для паттерна Хранитель).</summary>
    public DbSet<BookingSnapshot> BookingSnapshots => Set<BookingSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // email не должен повторяться
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
07:48


