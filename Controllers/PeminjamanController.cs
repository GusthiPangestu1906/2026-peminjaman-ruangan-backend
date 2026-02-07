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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Peminjaman>>> GetPeminjaman()
    {
        return await _context.Peminjamans.Include(p => p.Room).ToListAsync();
    }

    // LOGIKA 2: Collision Validation (Conflict Handling)
    [HttpPost]
    public async Task<ActionResult<Peminjaman>> PostPeminjaman(Peminjaman peminjaman)
    {
        // Sistem menolak jika RoomId dan TanggalPinjam bentrok dengan status 'Approved'
        var isConflict = await _context.Peminjamans
            .AnyAsync(p => p.RoomId == peminjaman.RoomId && 
                           p.TanggalPinjam.Date == peminjaman.TanggalPinjam.Date && 
                           p.Status == "Approved");

        if (isConflict)
        {
            return BadRequest(new { message = "Ruangan sudah ter-booking pada tanggal tersebut." });
        }

        _context.Peminjamans.Add(peminjaman);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPeminjaman), new { id = peminjaman.Id }, peminjaman);
    }

    // LOGIKA 1: Approval System (PATCH Method)
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var data = await _context.Peminjamans.FindAsync(id);
        if (data == null) return NotFound();

        // Validasi input status
        var validStatuses = new List<string> { "Pending", "Approved", "Rejected" };
        if (!validStatuses.Contains(newStatus)) 
            return BadRequest("Status tidak valid.");

        data.Status = newStatus;
        await _context.SaveChangesAsync();

        return NoContent(); // Mengembalikan 204 No Content
    }

    // GET: api/peminjaman/status/Pending
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Peminjaman>>> GetPeminjamanByStatus(string status)
    {
        // Menggunakan .ToLower() agar filter tidak sensitif terhadap huruf besar/kecil (Case Insensitive)
        var data = await _context.Peminjamans
            .Include(p => p.Room) // Tetap sertakan info ruangan sesuai AC
            .Where(p => p.Status.ToLower() == status.ToLower())
            .ToListAsync();

        // Jika data kosong, berikan pesan informatif
        if (data == null || data.Count == 0)
        {
            return NotFound(new { message = $"Tidak ada data peminjaman dengan status: {status}" });
        }

        return Ok(data);
    }
}