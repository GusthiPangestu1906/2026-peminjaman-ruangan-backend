using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_peminjaman_ruangan_backend.Models;

public class Peminjaman
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; } // Foreign Key

    // Navigasi ke tabel Room (agar bisa di-Include)
    [ForeignKey("RoomId")]
    public Room? Room { get; set; }

    [Required]
    public string Peminjam { get; set; } = string.Empty;

    [Required]
    public DateTime TanggalPinjam { get; set; }

    // FIX 1: Menambahkan kolom Keperluan yang menyebabkan error CS1061
    public string Keperluan { get; set; } = string.Empty;

    // FIX 2: Menambahkan kolom Status yang menyebabkan error CS1061
    // Default value "Pending" agar aman saat insert data baru
    public string Status { get; set; } = "Pending";
}