# Developer Guide & Troubleshooting

## 🚀 Quick Start

### 1. Setup Environment

```bash
# Clone repository (jika belum)
cd 2026-peminjaman-ruangan-backend

# Restore dependencies
dotnet restore

# Setup database
dotnet ef database update
```

### 2. Run Server

```bash
# Development mode dengan hot reload
dotnet watch run

# atau tanpa hot reload
dotnet run
```

Server akan berjalan di: **http://localhost:5215**

Swagger/API Explorer: **http://localhost:5215/swagger**

### 3. Test API

```bash
# Test dengan curl atau gunakan REST Client
# Contoh: lihat daftar peminjaman
curl http://localhost:5215/api/peminjaman
```

---

## 📁 Project Structure

```
2026-peminjaman-ruangan-backend/
├── Controllers/
│   ├── PeminjamanController.cs      ← Main CRUD endpoints
│   ├── RoomController.cs            ← Room management
│   └── CustomerController.cs        ← Customer management
│
├── Models/
│   ├── Peminjaman.cs               ← Entity model
│   ├── Room.cs                     ← Entity model
│   ├── Customer.cs                 ← Entity model
│   ├── PeminjamanDto.cs            ← Request DTO
│   ├── PeminjamanDetailDto.cs      ← Response DTO
│   ├── RoomDto.cs                  ← DTO untuk Room
│   ├── CustomerDto.cs              ← DTO untuk Customer
│   └── StatusUpdateDto.cs          ← DTO untuk status update
│
├── Data/
│   └── AppDbContext.cs             ← Entity Framework DbContext
│
├── Migrations/
│   ├── 20260206024931_InitialCreate.cs
│   ├── 20260206030628_SeedRoomsData.cs
│   ├── 20260206123808_AddPeminjamanTable.cs
│   ├── 20260209022307_UpdatePeminjamanModel.cs
│   ├── 20260209114022_AddCustomerAndSeeding.cs
│   └── 20260210060520_RestoreRoomsAndAddPeminjamanData.cs
│
├── Program.cs                      ← Application startup
├── appsettings.json               ← Configuration
├── appsettings.Development.json   ← Dev-specific config
├── 2026-peminjaman-ruangan-backend.csproj
├── API_DOCUMENTATION.md           ← API Reference
├── IMPLEMENTATION_CHECKLIST.md    ← Feature checklist
└── DEVELOPER_GUIDE.md             ← This file
```

---

## 🗄️ Database Schema

### Table: Rooms
```sql
CREATE TABLE "Rooms" (
    "Id" integer PRIMARY KEY,
    "Name" text NOT NULL,
    "Capacity" integer NOT NULL,
    "Location" text NOT NULL,
    "IsAvailable" boolean NOT NULL DEFAULT true
);
```

### Table: Peminjamans
```sql
CREATE TABLE "Peminjamans" (
    "Id" integer PRIMARY KEY,
    "RoomId" integer NOT NULL FOREIGN KEY,
    "Peminjam" text NOT NULL,
    "TanggalPinjam" timestamp with time zone NOT NULL,
    "Keperluan" text,
    "Status" text DEFAULT 'Pending'
);
```

### Table: Customers
```sql
CREATE TABLE "Customers" (
    "Id" integer PRIMARY KEY,
    "Name" text NOT NULL,
    "Email" text NOT NULL,
    "Phone" text
);
```

### Foreign Key Relationship
```
Peminjamans.RoomId → Rooms.Id
```

---

## 🔧 Common Development Tasks

### 1. Menambah Field Baru ke Peminjaman

**File 1: Models/Peminjaman.cs**
```csharp
public class Peminjaman
{
    // ... existing fields ...
    
    // Tambahkan field baru
    [Required]
    public string NoTelepon { get; set; } = string.Empty;
}
```

**File 2: Models/PeminjamanDto.cs**
```csharp
public class PeminjamanDto
{
    // ... existing fields ...
    
    [Required]
    public string NoTelepon { get; set; } = string.Empty;
}
```

**File 3: Models/PeminjamanDetailDto.cs**
```csharp
public class PeminjamanDetailDto
{
    // ... existing fields ...
    
    public string NoTelepon { get; set; } = string.Empty;
}
```

**File 4: Controllers/PeminjamanController.cs**
Tambahkan mapping di semua tempat yang menggunakan DTO:
```csharp
// Saat mapping dari Entity ke DTO
var resultDto = new PeminjamanDetailDto
{
    // ... existing mappings ...
    NoTelepon = peminjaman.NoTelepon
};
```

**File 5: Create Migration**
```bash
dotnet ef migrations add AddNoTeleponToPeminjaman
```

Migration akan otomatis detect field baru dan generate SQL.

**File 6: Apply Migration**
```bash
dotnet ef database update
```

---

### 2. Menambah Validasi Baru

**Contoh: Validasi email peminjam**

**File: Controllers/PeminjamanController.cs**
```csharp
[HttpPost]
public async Task<ActionResult<PeminjamanDetailDto>> PostPeminjaman(PeminjamanDto peminjamanDto)
{
    // ... existing validation ...
    
    // Tambahkan validasi email
    if (!IsValidEmail(peminjamanDto.Peminjam))
    {
        return BadRequest(new { message = "Format email tidak valid." });
    }
    
    // ... rest of method ...
}

private bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}
```

---

### 3. Menambah Query Parameter Baru

**Contoh: Filter berdasarkan date range**

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<PeminjamanDetailDto>>> GetPeminjaman(
    [FromQuery] string? search,
    [FromQuery] DateTime? dateFrom,
    [FromQuery] DateTime? dateTo)
{
    var query = _context.Peminjamans.Include(p => p.Room).AsQueryable();

    // Existing search logic
    if (!string.IsNullOrWhiteSpace(search))
    {
        search = search.ToLower();
        query = query.Where(p => p.Peminjam.ToLower().Contains(search) || 
                                 p.Keperluan.ToLower().Contains(search));
    }

    // Tambahkan date filter
    if (dateFrom.HasValue)
    {
        query = query.Where(p => p.TanggalPinjam.Date >= dateFrom.Value.Date);
    }

    if (dateTo.HasValue)
    {
        query = query.Where(p => p.TanggalPinjam.Date <= dateTo.Value.Date);
    }

    // ... rest of method ...
}
```

Usage:
```
GET /api/peminjaman?search=Rara&dateFrom=2026-02-10&dateTo=2026-02-15
```

---

## 🐛 Troubleshooting

### Issue 1: "address already in use" on port 5215

**Penyebab**: Server sudah berjalan di port tersebut

**Solusi**:
```powershell
# Cari proses yang menggunakan port
Get-NetTCPConnection -LocalPort 5215

# Kill process
Stop-Process -Id <PID> -Force

# atau ubah port di launchSettings.json
```

File: `Properties/launchSettings.json`
```json
{
    "profiles": {
        "http": {
            "commandName": "Project",
            "dotnetRunMessages": true,
            "launchBrowser": true,
            "applicationUrl": "http://localhost:5216",  ← Ubah port
            "environmentVariables": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        }
    }
}
```

---

### Issue 2: "database already exists" during migration

**Penyebab**: Database sudah ada, migration sudah di-apply

**Solusi**:
```bash
# Lihat migrations yang sudah diapply
dotnet ef migrations list

# Jika perlu reset database (HATI-HATI! Data akan hilang)
dotnet ef database drop
dotnet ef database update
```

---

### Issue 3: PostgreSQL tidak bisa connect

**Penyebab**: Connection string salah atau PostgreSQL belum running

**Solusi**:

**File: appsettings.Development.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=peminjaman_ruangan;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

Verifikasi:
```bash
# Cek apakah PostgreSQL running
# Ubah password sesuai konfigurasi lokal Anda
```

---

### Issue 4: DateTime error dari PostgreSQL

**Error Message**:
```
'timestamp with time zone' literal cannot be generated for Unspecified DateTime
```

**Penyebab**: DateTime harus dalam format UTC untuk PostgreSQL

**Solusi**:
```csharp
// WRONG
new DateTime(2026, 2, 11)

// CORRECT
new DateTime(2026, 2, 11, 0, 0, 0, DateTimeKind.Utc)

// atau
DateTime.UtcNow
```

---

### Issue 5: Foreign Key Constraint Violation

**Error Message**:
```
violation of FOREIGN KEY constraint "FK_Peminjamans_Rooms_RoomId"
```

**Penyebab**: RoomId referensi tidak ada di table Rooms

**Solusi**:
```bash
# Pastikan room dengan ID tersebut sudah ada
curl http://localhost:5215/api/room

# Gunakan RoomId yang ada dalam list (biasanya 1-5)
```

---

### Issue 6: Conflict pada update peminjaman

**Error Response**:
```json
{
  "message": "Ruangan sudah ter-booking (Approved) pada tanggal tersebut."
}
```

**Penyebab**: Ada peminjaman lain dengan status Approved pada tanggal yang sama

**Solusi**:
```bash
# Pilih tanggal yang berbeda atau ubah status peminjaman lain terlebih dahulu
curl -X PATCH http://localhost:5215/api/peminjaman/2/status \
  -H "Content-Type: application/json" \
  -d '{"status":"Rejected"}'
```

---

## 📝 API Testing dengan cURL

### 1. Create (POST)
```bash
curl -X POST http://localhost:5215/api/peminjaman \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "peminjam": "John Doe",
    "tanggalPinjam": "2026-02-15T10:00:00Z",
    "keperluan": "Meeting"
  }'
```

### 2. Read (GET)
```bash
# List semua
curl http://localhost:5215/api/peminjaman

# Search
curl http://localhost:5215/api/peminjaman?search=John

# Detail spesifik
curl http://localhost:5215/api/peminjaman/1

# Filter by status
curl http://localhost:5215/api/peminjaman/status/Pending
```

### 3. Update (PUT)
```bash
curl -X PUT http://localhost:5215/api/peminjaman/1 \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 2,
    "peminjam": "John Doe Updated",
    "tanggalPinjam": "2026-02-16T10:00:00Z",
    "keperluan": "Updated Meeting"
  }'
```

### 4. Update Status (PATCH)
```bash
curl -X PATCH http://localhost:5215/api/peminjaman/1/status \
  -H "Content-Type: application/json" \
  -d '{"status":"Approved"}'
```

### 5. Delete (DELETE)
```bash
curl -X DELETE http://localhost:5215/api/peminjaman/1
```

---

## 🧪 Testing Strategy

### Unit Testing (Recommended Implementation)
```csharp
// File: PeminjamanControllerTests.cs

[TestClass]
public class PeminjamanControllerTests
{
    private PeminjamanController _controller;
    private AppDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        // Setup in-memory database
        // Setup controller dengan mocked/test context
    }

    [TestMethod]
    public async Task PostPeminjaman_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new PeminjamanDto 
        { 
            RoomId = 1, 
            Peminjam = "Test", 
            TanggalPinjam = DateTime.UtcNow.AddDays(1),
            Keperluan = "Test" 
        };

        // Act
        var result = await _controller.PostPeminjaman(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public async Task PostPeminjaman_WithPastDate_ReturnsBadRequest()
    {
        // Test untuk validasi tanggal masa lalu
    }

    [TestMethod]
    public async Task PostPeminjaman_WithConflictingBooking_ReturnsBadRequest()
    {
        // Test conflict detection
    }
}
```

---

## 📚 Code Review Checklist

Sebelum commit, pastikan:

- [ ] Semua DTOs ter-update jika ada perubahan Entity
- [ ] Semua method controller ada validasi input
- [ ] Error messages informatif dan berguna
- [ ] Foreign keys tidak violated
- [ ] DateTime menggunakan UTC
- [ ] Include() digunakan untuk eager loading
- [ ] Async/await digunakan
- [ ] Database updated sebelum testing
- [ ] API tested dengan berbagai parameter

---

## 🔗 Useful Commands

```bash
# Lihat semua migrations
dotnet ef migrations list

# Undo migration yang belum diapply
dotnet ef migrations remove

# Lihat SQL dari migration
dotnet ef migrations script <migration-name>

# Delete & recreate database
dotnet ef database drop
dotnet ef database update

# Open SQL CLI untuk PostgreSQL
psql -U postgres -d peminjaman_ruangan

# Restore packages
dotnet restore

# Clean build artifacts
dotnet clean

# Format code
dotnet format
```

---

## 📖 Project Dependencies

Cek [2026-peminjaman-ruangan-backend.csproj](2026-peminjaman-ruangan-backend.csproj):

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="..." />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="..." />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="..." />
</ItemGroup>
```

Update packages jika diperlukan:
```bash
dotnet package update

# Update specific package
dotnet package update Microsoft.EntityFrameworkCore
```

---

## 🚀 Deployment Preparation

Sebelum production:

1. **Validate Configuration**
   ```bash
   # Cek apakah appsettings.json valid
   dotnet run
   ```

2. **Run Migrations**
   ```bash
   dotnet ef database update
   ```

3. **Test All Endpoints**
   - [ ] GET /api/peminjaman
   - [ ] POST /api/peminjaman
   - [ ] PUT /api/peminjaman/{id}
   - [ ] PATCH /api/peminjaman/{id}/status
   - [ ] DELETE /api/peminjaman/{id}
   - [ ] GET /api/peminjaman/status/{status}

4. **Security Check**
   - [ ] ConnectionString tidak di-hardcode
   - [ ] Use environment variables untuk sensitive data
   - [ ] Validate all user inputs
   - [ ] Add authentication (jika needed)

5. **Performance Check**
   - [ ] Database queries optimized dengan Include()
   - [ ] No N+1 query problems
   - [ ] Pagination implemented (untuk large datasets)

---

## 📚 Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core REST API Best Practices](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [PostgreSQL Official Docs](https://www.postgresql.org/docs/)

