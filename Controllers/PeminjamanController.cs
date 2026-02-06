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

    [HttpGet] // Melihat daftar
    public async Task<ActionResult<IEnumerable<Peminjaman>>> GetPeminjaman() {
        return await _context.Peminjamans.Include(p => p.Room).ToListAsync();
    }

    [HttpPost] // Menambah data
    public async Task<ActionResult<Peminjaman>> PostPeminjaman(Peminjaman peminjaman) {
        _context.Peminjamans.Add(peminjaman);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPeminjaman), new { id = peminjaman.Id }, peminjaman);
    }
}