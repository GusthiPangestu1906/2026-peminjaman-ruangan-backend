using System.ComponentModel.DataAnnotations;

namespace _2026_peminjaman_ruangan_backend.Models;

public class Peminjaman {
    public int Id { get; set; }
    [Required]
    public int RoomId { get; set; } 
    [Required]
    public string Peminjam { get; set; } = string.Empty;
    [Required]
    public DateTime TanggalPinjam { get; set; }
    public string Status { get; set; } = "Pending"; // Sesuai PdBL hal. 7 [cite: 87]
    public Room? Room { get; set; } // Navigation property
}