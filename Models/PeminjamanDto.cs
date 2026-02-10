using System.ComponentModel.DataAnnotations;

namespace _2026_peminjaman_ruangan_backend.Models;

public class PeminjamanDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public string Peminjam { get; set; } = string.Empty;

    [Required]
    public DateTime TanggalPinjam { get; set; }

    public string Keperluan { get; set; } = string.Empty;
}