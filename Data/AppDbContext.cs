using Microsoft.EntityFrameworkCore;
using _2026_peminjaman_ruangan_backend.Models;

namespace _2026_peminjaman_ruangan_backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Peminjaman> Peminjamans { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Data Seeding
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Budi Santoso", Email = "budi@example.com", Phone = "081234567890" },
            new Customer { Id = 2, Name = "Siti Aminah", Email = "siti@example.com", Phone = "089876543210" }
        );
    }
}