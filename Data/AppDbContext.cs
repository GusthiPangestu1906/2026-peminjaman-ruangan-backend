using Microsoft.EntityFrameworkCore;
using _2026_peminjaman_ruangan_backend.Models;

namespace _2026_peminjaman_ruangan_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Ruang Teori 1", Capacity = 30, Location = "Gedung D3", IsAvailable = true },
                new Room { Id = 2, Name = "Lab Informatika", Capacity = 25, Location = "Gedung D4", IsAvailable = true },
                new Room { Id = 3, Name = "Aula Pens", Capacity = 100, Location = "Gedung TC", IsAvailable = true },
                new Room { Id = 4, Name = "Ruang Rapat", Capacity = 15, Location = "Gedung Pusat", IsAvailable = false },
                new Room { Id = 5, Name = "Theater PENS", Capacity = 50, Location = "Gedung Pasca", IsAvailable = true }
            );
        }
    }
}