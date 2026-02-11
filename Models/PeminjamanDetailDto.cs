namespace _2026_peminjaman_ruangan_backend.Models;

public class PeminjamanDetailDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public RoomDto? Room { get; set; }
    public string Peminjam { get; set; } = string.Empty;
    public DateTime TanggalPinjam { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Keperluan { get; set; } = string.Empty;
}