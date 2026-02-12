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
    public async Task<ActionResult<IEnumerable<PeminjamanDetailDto>>> GetPeminjaman([FromQuery] string? search)
    {
        var query = _context.Peminjamans.Include(p => p.Room).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p => p.Peminjam.ToLower().Contains(search) || 
                                     p.Keperluan.ToLower().Contains(search));
        }

        var data = await query.ToListAsync();
        
        var dtos = data.Select(p => new PeminjamanDetailDto
        {
            Id = p.Id,
            RoomId = p.RoomId,
            Room = p.Room == null ? null : new RoomDto 
            {
                Id = p.Room.Id,
                Name = p.Room.Name,
                Capacity = p.Room.Capacity,
                Location = p.Room.Location,
                IsAvailable = p.Room.IsAvailable
            },
            Peminjam = p.Peminjam,
            TanggalPinjam = p.TanggalPinjam,
            Status = p.Status,
            Keperluan = p.Keperluan
        }).ToList();

        return Ok(dtos);
    }

    // POST: api/peminjaman
    // Menambah peminjaman baru dengan validasi data dan cek bentrok jadwal
    [HttpPost]
    public async Task<ActionResult<PeminjamanDetailDto>> PostPeminjaman(PeminjamanDto peminjamanDto)
    {
        // 1. Validasi Input: Pastikan Nama dan Keperluan tidak kosong
        // Note: RoomId <= 0 diasumsikan tidak valid karena ID database biasanya mulai dari 1
        if (string.IsNullOrWhiteSpace(peminjamanDto.Peminjam) || 
            peminjamanDto.RoomId <= 0)
        {
            return BadRequest(new { message = "Data tidak lengkap. Nama Peminjam dan RoomId wajib diisi." });
        }

        // Validasi Tanggal: Tidak boleh masa lalu
        if (peminjamanDto.TanggalPinjam < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Tanggal peminjaman tidak boleh di masa lalu." });
        }

        // 2. Logic Limit: Satu user maksimal 2 booking per hari
        // Hitung booking user ini di tanggal yang sama (kecuali yang Rejected)
        var userBookingCount = await _context.Peminjamans
            .CountAsync(p => p.Peminjam.ToLower() == peminjamanDto.Peminjam.ToLower() && 
                             p.TanggalPinjam.Date == peminjamanDto.TanggalPinjam.Date &&
                             p.Status != "Rejected");

        if (userBookingCount >= 2)
        {
            return BadRequest(new { message = $"Gagal: User {peminjamanDto.Peminjam} sudah mencapai batas maksimal 2 peminjaman pada tanggal tersebut." });
        }

        // 3. Logic Collision: Cek apakah ruangan sudah dipinjam (Status != Rejected)
        // Artinya Pending dan Approved dianggap memblokir ruangan, hanya Rejected yang membebaskan ruangan.
        var conflictingBooking = await _context.Peminjamans
            .Include(p => p.Room)
            .FirstOrDefaultAsync(p => p.RoomId == peminjamanDto.RoomId && 
                           p.TanggalPinjam.Date == peminjamanDto.TanggalPinjam.Date && 
                           p.Status != "Rejected");

        if (conflictingBooking != null)
        {
            return BadRequest(new { message = $"Gagal: Ruangan {conflictingBooking.Room?.Name} sudah dibooking oleh {conflictingBooking.Peminjam} (Status: {conflictingBooking.Status})." });
        }

        // Mapping DTO ke Entity
        var peminjaman = new Peminjaman
        {
            RoomId = peminjamanDto.RoomId,
            Peminjam = peminjamanDto.Peminjam,
            TanggalPinjam = peminjamanDto.TanggalPinjam,
            Keperluan = peminjamanDto.Keperluan,
            Status = "Pending" // Default status
        };

        _context.Peminjamans.Add(peminjaman);
        await _context.SaveChangesAsync();

        // Load Room untuk response
        await _context.Entry(peminjaman).Reference(p => p.Room).LoadAsync();

        var resultDto = new PeminjamanDetailDto
        {
            Id = peminjaman.Id,
            RoomId = peminjaman.RoomId,
            Room = peminjaman.Room == null ? null : new RoomDto
            {
                Id = peminjaman.Room.Id,
                Name = peminjaman.Room.Name,
                Capacity = peminjaman.Room.Capacity,
                Location = peminjaman.Room.Location,
                IsAvailable = peminjaman.Room.IsAvailable
            },
            Peminjam = peminjaman.Peminjam,
            TanggalPinjam = peminjaman.TanggalPinjam,
            Status = peminjaman.Status,
            Keperluan = peminjaman.Keperluan
        };

        // Mengembalikan respons 201 Created beserta lokasi data baru
        return CreatedAtAction(nameof(GetPeminjaman), new { id = peminjaman.Id }, resultDto);
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
    public async Task<ActionResult<IEnumerable<PeminjamanDetailDto>>> GetPeminjamanByStatus(string status)
    {
        var data = await _context.Peminjamans
            .Include(p => p.Room)
            .Where(p => p.Status.ToLower() == status.ToLower())
            .ToListAsync();

        if (data == null || data.Count == 0)
        {
            return NotFound(new { message = $"Tidak ada data peminjaman dengan status: {status}" });
        }

        var dtos = data.Select(p => new PeminjamanDetailDto
        {
            Id = p.Id,
            RoomId = p.RoomId,
            Room = p.Room == null ? null : new RoomDto
            {
                Id = p.Room.Id,
                Name = p.Room.Name,
                Capacity = p.Room.Capacity,
                Location = p.Room.Location,
                IsAvailable = p.Room.IsAvailable
            },
            Peminjam = p.Peminjam,
            TanggalPinjam = p.TanggalPinjam,
            Status = p.Status,
            Keperluan = p.Keperluan
        }).ToList();

        return Ok(dtos);
    }

    // GET: api/peminjaman/{id}
    // Melihat detail satu peminjaman
    [HttpGet("{id}")]
    public async Task<ActionResult<PeminjamanDetailDto>> GetPeminjaman(int id)
    {
        var peminjaman = await _context.Peminjamans.Include(p => p.Room).FirstOrDefaultAsync(p => p.Id == id);

        if (peminjaman == null)
        {
            return NotFound();
        }

        var dto = new PeminjamanDetailDto
        {
            Id = peminjaman.Id,
            RoomId = peminjaman.RoomId,
            Room = peminjaman.Room == null ? null : new RoomDto
            {
                Id = peminjaman.Room.Id,
                Name = peminjaman.Room.Name,
                Capacity = peminjaman.Room.Capacity,
                Location = peminjaman.Room.Location,
                IsAvailable = peminjaman.Room.IsAvailable
            },
            Peminjam = peminjaman.Peminjam,
            TanggalPinjam = peminjaman.TanggalPinjam,
            Status = peminjaman.Status,
            Keperluan = peminjaman.Keperluan
        };

        return dto;
    }

    // PUT: api/peminjaman/{id}
    // Mengubah data peminjaman (Edit Full)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPeminjaman(int id, PeminjamanDto peminjamanDto)
    {
        var peminjaman = await _context.Peminjamans.FindAsync(id);
        if (peminjaman == null) return NotFound();

        // Validasi Tanggal: Tidak boleh masa lalu
        if (peminjamanDto.TanggalPinjam < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Tanggal peminjaman tidak boleh di masa lalu." });
        }

        // Logic Limit: Cek limit user per hari (abaikan diri sendiri saat edit)
        var userBookingCount = await _context.Peminjamans
            .CountAsync(p => p.Id != id && 
                             p.Peminjam.ToLower() == peminjamanDto.Peminjam.ToLower() && 
                             p.TanggalPinjam.Date == peminjamanDto.TanggalPinjam.Date &&
                             p.Status != "Rejected");

        if (userBookingCount >= 2)
        {
            return BadRequest(new { message = $"Gagal mengubah: User {peminjamanDto.Peminjam} sudah mencapai batas maksimal 2 peminjaman pada tanggal tersebut." });
        }

        // Logic Collision: Cek apakah ruangan sudah dipinjam orang lain (Status != Rejected)
        var conflictingBooking = await _context.Peminjamans
            .Include(p => p.Room)
            .FirstOrDefaultAsync(p => p.Id != id && // Penting: Abaikan data diri sendiri saat pengecekan
                           p.RoomId == peminjamanDto.RoomId && 
                           p.TanggalPinjam.Date == peminjamanDto.TanggalPinjam.Date && 
                           p.Status != "Rejected");

        if (conflictingBooking != null)
        {
            return BadRequest(new { message = $"Gagal mengubah: Ruangan {conflictingBooking.Room?.Name} sudah dibooking oleh {conflictingBooking.Peminjam} (Status: {conflictingBooking.Status})." });
        }

        // Update field yang diperbolehkan saja
        peminjaman.RoomId = peminjamanDto.RoomId;
        peminjaman.Peminjam = peminjamanDto.Peminjam;
        peminjaman.TanggalPinjam = peminjamanDto.TanggalPinjam;
        peminjaman.Keperluan = peminjamanDto.Keperluan;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Peminjamans.Any(e => e.Id == id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // DELETE: api/peminjaman/{id}
    // Menghapus data peminjaman
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePeminjaman(int id)
    {
        var peminjaman = await _context.Peminjamans.FindAsync(id);
        if (peminjaman == null) return NotFound();

        _context.Peminjamans.Remove(peminjaman);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}