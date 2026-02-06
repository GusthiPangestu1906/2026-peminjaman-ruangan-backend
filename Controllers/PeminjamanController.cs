[Route("api/[controller]")]
[ApiController]
public class PeminjamanController : ControllerBase {
    private readonly AppDbContext _context;
    public PeminjamanController(AppDbContext context) { _context = context; }

    [cite_start][HttpGet] // Melihat daftar [cite: 83]
    public async Task<ActionResult<IEnumerable<Peminjaman>>> GetPeminjaman() {
        return await _context.Peminjamans.Include(p => p.Room).ToListAsync();
    }

    [cite_start][HttpPost] // Menambah data [cite: 82]
    public async Task<ActionResult<Peminjaman>> PostPeminjaman(Peminjaman peminjaman) {
        _context.Peminjamans.Add(peminjaman);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPeminjaman), new { id = peminjaman.Id }, peminjaman);
    }
}