using Microsoft.EntityFrameworkCore;
using _2026_peminjaman_ruangan_backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. DAFTARKAN SERVICES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. KONFIGURASI DATABASE (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. KONFIGURASI CORS (PENTING: Agar Frontend bisa akses API)
builder.Services.AddCors();

var app = builder.Build();

// 4. KONFIGURASI HTTP REQUEST PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// URUTAN INI SANGAT PENTING!
// app.UseHttpsRedirection();

// Aktifkan CORS sebelum Authorization
app.UseCors(corsBuilder => corsBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();