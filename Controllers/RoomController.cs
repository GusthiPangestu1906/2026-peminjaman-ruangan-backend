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
            Location = r.Location
        }).ToList();

        return Ok(dtos);
    }
}