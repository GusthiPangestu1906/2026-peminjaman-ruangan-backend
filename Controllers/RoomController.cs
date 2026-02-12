using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2026_peminjaman_ruangan_backend.Data;
using _2026_peminjaman_ruangan_backend.Models;

namespace _2026_peminjaman_ruangan_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/room
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
    {
        var rooms = await _context.Rooms.ToListAsync();
        var dtos = rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Location = r.Location,
            IsAvailable = r.IsAvailable
        }).ToList();

        return Ok(dtos);
    }

    // GET: api/room/available?date=2026-02-15&capacity=30&roomName=Aula
    // Cek ketersediaan ruangan pada tanggal tertentu, kapasitas (opsional), dan nama ruangan (opsional)
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms([FromQuery] DateTime date, [FromQuery] int? capacity, [FromQuery] string? roomName)
    {
        // Validasi: Tanggal tidak boleh default atau masa lalu
        if (date == default(DateTime) || date.Date < DateTime.UtcNow.Date)
        {
            return BadRequest(new { message = "Tanggal wajib diisi dan tidak boleh di masa lalu." });
        }

        // 1. Ambil ID ruangan yang sudah dibooking (Pending/Approved) pada tanggal tersebut
        var bookedRoomIds = await _context.Peminjamans
            .Where(p => p.TanggalPinjam.Date == date.Date && p.Status != "Rejected")
            .Select(p => p.RoomId)
            .ToListAsync();

        // 2. Filter ruangan:
        // - Harus Available secara umum (IsAvailable == true)
        // - TIDAK ada di daftar bookedRoomIds
        // - Kapasitas cukup (jika parameter capacity diisi)
        // - Nama sesuai (jika parameter roomName diisi)
        var query = _context.Rooms
            .Where(r => r.IsAvailable && !bookedRoomIds.Contains(r.Id));

        if (capacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= capacity.Value);
        }

        if (!string.IsNullOrWhiteSpace(roomName))
        {
            // Pencarian case-insensitive (mengandung kata kunci)
            query = query.Where(r => r.Name.ToLower().Contains(roomName.ToLower()));
        }

        var availableRooms = await query.ToListAsync();

        var dtos = availableRooms.Select(r => new RoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Location = r.Location,
            IsAvailable = r.IsAvailable
        }).ToList();

        return Ok(dtos);
    }

    // GET: api/room/1/bookings
    // Mendapatkan daftar tanggal di mana ruangan sudah dibooking (Pending/Approved)
    [HttpGet("{id}/bookings")]
    public async Task<ActionResult> GetRoomBookings(int id)
    {
        // Gunakan UtcNow untuk menghindari error PostgreSQL (timestamp with time zone)
        var today = DateTime.UtcNow.Date;

        var bookings = await _context.Peminjamans
            .Where(p => p.RoomId == id && p.Status != "Rejected" && p.TanggalPinjam >= today)
            .OrderBy(p => p.TanggalPinjam)
            .Select(p => new
            {
                p.TanggalPinjam,
                p.Peminjam,
                p.Keperluan
            })
            .ToListAsync();

        return Ok(bookings);
    }
}