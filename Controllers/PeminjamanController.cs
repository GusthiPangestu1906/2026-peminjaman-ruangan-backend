using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2026_peminjaman_ruangan_backend.Data;
using _2026_peminjaman_ruangan_backend.Models;

namespace _2026_peminjaman_ruangan_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeminjamanController : ControllerBase
{
    private readonly AppDbContext _context;

    public PeminjamanController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/peminjaman
    // Mengambil semua data peminjaman beserta detail ruangannya
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Peminjaman>>> GetPeminjaman()
    {
        return await _context.Peminjamans.Include(p => p.Room).ToListAsync();
    }

    // POST: api/peminjaman
    // Menambah peminjaman baru dengan validasi data dan cek bentrok jadwal
    [HttpPost]
    public async Task<ActionResult<Peminjaman>> PostPeminjaman(Peminjaman peminjaman)
    {
        // 1. Validasi Input: Pastikan Nama dan Keperluan tidak kosong
        // Note: RoomId <= 0 diasumsikan tidak valid karena ID database biasanya mulai dari 1
        if (string.IsNullOrWhiteSpace(peminjaman.Peminjam) || 
            // string.IsNullOrWhiteSpace(peminjaman.Keperluan) || // Aktifkan jika ada field Keperluan
            peminjaman.RoomId <= 0)
        {
            return BadRequest(new { message = "Data tidak lengkap. Nama Peminjam dan RoomId wajib diisi." });
        }

        // 2. Security: Paksa status default jadi "Pending"
        // Mencegah user mengirim status "Approved" langsung dari API client
        peminjaman.Status = "Pending";

        // 3. Logic Collision: Cek apakah ruangan sudah dipinjam di tanggal yang sama dengan status Approved
        var isConflict = await _context.Peminjamans
            .AnyAsync(p => p.RoomId == peminjaman.RoomId && 
                           p.TanggalPinjam.Date == peminjaman.TanggalPinjam.Date && 
                           p.Status == "Approved");

        if (isConflict)
        {
            return BadRequest(new { message = "Ruangan sudah ter-booking (Approved) pada tanggal tersebut." });
        }

        _context.Peminjamans.Add(peminjaman);
        await _context.SaveChangesAsync();

        // Mengembalikan respons 201 Created beserta lokasi data baru
        return CreatedAtAction(nameof(GetPeminjaman), new { id = peminjaman.Id }, peminjaman);
    }

    // PATCH: api/peminjaman/{id}/status
    // Mengubah status peminjaman (Approve/Reject)
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
    {
        // Validasi payload body tidak boleh kosong
        if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            return BadRequest("Status tidak valid.");

        var newStatus = dto.Status;
        var data = await _context.Peminjamans.FindAsync(id);
        
        if (data == null) return NotFound();

        // Validasi nilai status yang diperbolehkan
        var validStatuses = new List<string> { "Pending", "Approved", "Rejected" };
        if (!validStatuses.Contains(newStatus)) 
            return BadRequest("Status harus berupa: Pending, Approved, atau Rejected.");

        data.Status = newStatus;
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content (Sukses tanpa body)
    }

    // GET: api/peminjaman/status/{status}
    // Filter data berdasarkan status (Case Insensitive)
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Peminjaman>>> GetPeminjamanByStatus(string status)
    {
        var data = await _context.Peminjamans
            .Include(p => p.Room)
            .Where(p => p.Status.ToLower() == status.ToLower())
            .ToListAsync();

        if (data == null || data.Count == 0)
        {
            return NotFound(new { message = $"Tidak ada data peminjaman dengan status: {status}" });
        }

        return Ok(data);
    }
}