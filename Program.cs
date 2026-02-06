using Microsoft.EntityFrameworkCore;
using _2026_peminjaman_ruangan_backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Tambahkan dukungan untuk Controller agar RoomController bisa dideteksi
builder.Services.AddControllers(); 

// 2. Konfigurasi Swagger/OpenAPI
builder.Services.AddOpenApi();

// 3. Konfigurasi Database PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 4. Konfigurasi HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 5. Penting: MapControllers agar rute [Route("api/[controller]")] bisa diproses
app.MapControllers(); 

app.Run();