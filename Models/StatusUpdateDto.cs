namespace _2026_peminjaman_ruangan_backend.Models;

// Class kecil ini berfungsi untuk menangkap data JSON saat PATCH status
public class StatusUpdateDto
{
    public string Status { get; set; } = string.Empty;
}