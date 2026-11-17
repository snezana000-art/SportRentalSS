using Microsoft.EntityFrameworkCore;
using SportRental.Models;

namespace SportRental.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Equipment> Inventory => Set<Equipment>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSnapshot> BookingSnapshots => Set<BookingSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // email не должен повторяться
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
