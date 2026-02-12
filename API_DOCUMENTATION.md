# API Documentation - Sistem Peminjaman Ruangan

## Status Implementasi ✅

Sistem telah mengimplementasikan **semua requirement** yang diminta:

### 1. Pencatatan Peminjaman Ruangan ✅
- ✅ Menambah data peminjaman
- ✅ Melihat daftar peminjaman
- ✅ Melihat detail peminjaman
- ✅ Mengubah data peminjaman
- ✅ Menghapus data peminjaman

### 2. Pengelolaan Status Peminjaman ✅
- ✅ Melihat status peminjaman
- ✅ Mengubah status peminjaman (Pending → Approved/Rejected)
- ℹ️ Menyimpan riwayat perubahan status (Opsional - belum diimplementasikan)

### 3. Riwayat dan Penelusuran Peminjaman ✅
- ✅ Menampilkan riwayat peminjaman (GET daftar)
- ✅ Pencarian data peminjaman (berdasarkan nama/keperluan)
- ✅ Filter data berdasarkan status
- ✅ Load data dengan detail ruangan (Include relation)

### 4. Struktur Kode (Separation of Concerns) ✅
- ✅ Entity Classes (Models)
- ✅ Data Transfer Objects (DTOs)
- ✅ Database Context
- ✅ Controllers
- ✅ Migrations
- ✅ Data Seeding

---

## API Endpoints

### **PEMINJAMAN (Peminjaman Controller)**

#### 1. GET - Daftar Semua Peminjaman
```http
GET /api/peminjaman
GET /api/peminjaman?search=nama_peminjam
```
**Query Parameters:**
- `search` (optional): Cari berdasarkan nama peminjam atau keperluan

**Response (200 OK):**
```json
{
  "value": [
    {
      "id": 1,
      "roomId": 1,
      "room": {
        "id": 1,
        "name": "Ruang Teori 1",
        "capacity": 30,
        "location": "Gedung D3"
      },
      "peminjam": "Gusthi Pangestu",
      "tanggalPinjam": "2026-02-11T00:00:00Z",
      "status": "Approved",
      "keperluan": "Rapat Koordinasi Tim"
    }
  ],
  "count": 4
}
```

**Status Code:**
- 200 OK: Berhasil mengambil data
- 404 Not Found: Tidak ada data

---

#### 2. GET - Detail Peminjaman Spesifik
```http
GET /api/peminjaman/{id}
```
**Path Parameters:**
- `id` (required): ID peminjaman

**Response (200 OK):**
```json
{
  "id": 1,
  "roomId": 1,
  "room": {
    "id": 1,
    "name": "Ruang Teori 1",
    "capacity": 30,
    "location": "Gedung D3"
  },
  "peminjam": "Gusthi Pangestu",
  "tanggalPinjam": "2026-02-11T00:00:00Z",
  "status": "Approved",
  "keperluan": "Rapat Koordinasi Tim"
}
```

**Status Code:**
- 200 OK: Berhasil
- 404 Not Found: Peminjaman tidak ditemukan

---

#### 3. POST - Buat Peminjaman Baru
```http
POST /api/peminjaman
Content-Type: application/json
```
**Request Body:**
```json
{
  "roomId": 2,
  "peminjam": "Rara Amelia",
  "tanggalPinjam": "2026-02-15T10:00:00Z",
  "keperluan": "Lab Praktikum Database"
}
```

**Validasi:**
- ✅ Nama peminjam tidak boleh kosong
- ✅ RoomId harus valid
- ✅ Tanggal tidak boleh di masa lalu
- ✅ **Limit**: User maksimal melakukan 2 booking per hari
- ✅ **Conflict**: Ruangan tidak boleh ter-booking (Pending/Approved) di tanggal yang sama

**Response (201 Created):**
```json
{
  "id": 5,
  "roomId": 2,
  "room": { /* room detail */ },
  "peminjam": "Rara Amelia",
  "tanggalPinjam": "2026-02-15T10:00:00Z",
  "status": "Pending",
  "keperluan": "Lab Praktikum Database"
}
```

**Status Code:**
- 201 Created: Berhasil membuat peminjaman
- 400 Bad Request: Validasi gagal
- 400 Bad Request: Ruangan conflict atau limit user tercapai

---

#### 4. PUT - Update Peminjaman Lengkap
```http
PUT /api/peminjaman/{id}
Content-Type: application/json
```
**Path Parameters:**
- `id` (required): ID peminjaman

**Request Body:**
```json
{
  "roomId": 3,
  "peminjam": "Rara Amelia",
  "tanggalPinjam": "2026-02-16T10:00:00Z",
  "keperluan": "Lab Praktikum Database Terbaru"
}
```

**Validasi:**
- ✅ Sama seperti POST (tanggal & booking conflict)
- ✅ Mengabaikan peminjaman diri sendiri saat cek conflict

**Response:**
- 204 No Content: Berhasil update

**Status Code:**
- 204 No Content: Berhasil
- 404 Not Found: Peminjaman tidak ditemukan
- 400 Bad Request: Validasi gagal

---

#### 5. PATCH - Update Status Peminjaman
```http
PATCH /api/peminjaman/{id}/status
Content-Type: application/json
```
**Path Parameters:**
- `id` (required): ID peminjaman

**Request Body:**
```json
{
  "status": "Approved"
}
```

**Status yang Valid:**
- `Pending` (default)
- `Approved` (disetujui)
- `Rejected` (ditolak)

**Response:**
- 204 No Content: Berhasil update status

**Status Code:**
- 204 No Content: Berhasil
- 404 Not Found: Peminjaman tidak ditemukan
- 400 Bad Request: Status tidak valid

---

#### 6. GET - Filter Peminjaman Berdasarkan Status
```http
GET /api/peminjaman/status/{status}
```
**Path Parameters:**
- `status` (required): Pending | Approved | Rejected

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "roomId": 1,
    "room": { /* room detail */ },
    "peminjam": "Gusthi Pangestu",
    "tanggalPinjam": "2026-02-11T00:00:00Z",
    "status": "Approved",
    "keperluan": "Rapat Koordinasi Tim"
  }
]
```

**Status Code:**
- 200 OK: Berhasil
- 404 Not Found: Tidak ada data dengan status tersebut

---

#### 7. DELETE - Hapus Peminjaman
```http
DELETE /api/peminjaman/{id}
```
**Path Parameters:**
- `id` (required): ID peminjaman

**Response:**
- 204 No Content: Berhasil dihapus

**Status Code:**
- 204 No Content: Berhasil
- 404 Not Found: Peminjaman tidak ditemukan

---

### **RUANGAN (Room Controller)**

#### 1. GET - Daftar Semua Ruangan
```http
GET /api/room
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Ruang Teori 1",
    "capacity": 30,
    "location": "Gedung D3",
    "isAvailable": true
  }
]
```

---

## Model Struktur

### Entity: Peminjaman
```csharp
public class Peminjaman
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public string Peminjam { get; set; }
    public DateTime TanggalPinjam { get; set; }
    public string Keperluan { get; set; }
    public string Status { get; set; } = "Pending";
}
```

### Entity: Room
```csharp
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
    public bool IsAvailable { get; set; } = true;
}
```

### Entity: Customer
```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
```

### DTO: PeminjamanDetailDto (Response)
```csharp
public class PeminjamanDetailDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public RoomDto? Room { get; set; }
    public string Peminjam { get; set; }
    public DateTime TanggalPinjam { get; set; }
    public string Status { get; set; }
    public string Keperluan { get; set; }
}
```

### DTO: PeminjamanDto (Request)
```csharp
public class PeminjamanDto
{
    public int RoomId { get; set; }
    public string Peminjam { get; set; }
    public DateTime TanggalPinjam { get; set; }
    public string Keperluan { get; set; }
}
```

### DTO: StatusUpdateDto (Request)
```csharp
public class StatusUpdateDto
{
    public string Status { get; set; }
}
```

### DTO: RoomDto (Response)
```csharp
public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
}
```

---

## Database Seeding

### Data Awal (Migration: RestoreRoomsAndAddPeminjamanData)

**5 Ruangan:**
| ID | Name | Capacity | Location | IsAvailable |
|---|---|---|---|---|
| 1 | Ruang Teori 1 | 30 | Gedung D3 | true |
| 2 | Lab Informatika | 25 | Gedung D4 | true |
| 3 | Aula Pens | 100 | Gedung TC | true |
| 4 | Ruang Rapat | 15 | Gedung Pusat | false |
| 5 | Theater PENS | 50 | Gedung Pasca | true |

**4 Peminjaman Sample:**
| ID | Peminjam | Ruangan | Tanggal | Status | Keperluan |
|---|---|---|---|---|---|
| 1 | Gusthi Pangestu | Ruang Teori 1 | 2026-02-11 | Approved | Rapat Koordinasi Tim |
| 2 | Rara Amelia | Lab Informatika | 2026-02-12 | Pending | Lab Praktikum Sistem Basis Data |
| 3 | Andi Wijaya | Aula Pens | 2026-02-13 | Approved | Seminar Teknologi Cloud |
| 4 | Siti Aminah | Theater PENS | 2026-02-14 | Pending | Workshop Web Development |

**2 Customer:**
| ID | Name | Email | Phone |
|---|---|---|---|
| 1 | Budi Santoso | budi@example.com | 081234567890 |
| 2 | Siti Aminah | siti@example.com | 089876543210 |

---

## Fitur Validasi & Bisnis Logic

### 1. Validasi Input
- ✅ Nama peminjam tidak boleh kosong
- ✅ RoomId harus valid (> 0)
- ✅ Tanggal peminjaman tidak boleh masa lalu

### 2. Booking Conflict Detection
- ✅ Cek apakah ruangan sudah di-book oleh orang lain pada tanggal yang sama
- ✅ Hanya pengecekan terhadap peminjaman dengan status "Approved"
- ✅ Mengabaikan peminjaman diri sendiri saat update

### 3. Status Management
- ✅ Status default: "Pending"
- ✅ Dapat diubah menjadi: "Approved" atau "Rejected"
- ✅ Validasi nilai status

### 4. Search & Filter
- ✅ Pencarian berdasarkan nama peminjam atau keperluan (case-insensitive)
- ✅ Filter berdasarkan status
- ✅ Eager loading data ruangan (Include)

### 5. Error Handling
- ✅ Mengembalikan error 400 untuk validasi gagal
- ✅ Mengembalikan error 404 untuk data tidak ditemukan
- ✅ Mengembalikan error 409 untuk booking conflict

---

## Teknologi yang Digunakan

- **Backend**: ASP.NET Core (C#)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **API Pattern**: RESTful API
- **Architecture**: Repository pattern dengan Separated Concerns

---

## File Struktur

```
Controllers/
├── PeminjamanController.cs       (7 endpoints CRUD + filter)
├── RoomController.cs             (GET all rooms)
└── CustomerController.cs

Models/
├── Peminjaman.cs                (Entity dengan FK ke Room)
├── Room.cs
├── Customer.cs
├── PeminjamanDto.cs             (Request DTO)
├── PeminjamanDetailDto.cs       (Response DTO dengan Room detail)
├── RoomDto.cs
├── CustomerDto.cs
└── StatusUpdateDto.cs           (Status update request)

Data/
└── AppDbContext.cs              (DbContext dengan DbSets)

Migrations/
├── 20260206024931_InitialCreate.cs
├── 20260206030628_SeedRoomsData.cs
├── 20260206123808_AddPeminjamanTable.cs
├── 20260209022307_UpdatePeminjamanModel.cs
├── 20260209114022_AddCustomerAndSeeding.cs
└── 20260210060520_RestoreRoomsAndAddPeminjamanData.cs (Data seeding)
```

---

## Penggunaan

### Setup Database
```bash
# Apply migrations
dotnet ef database update

# Lihat status migration
dotnet ef migrations list
```

### Run Server
```bash
dotnet run
# Server berjalan di http://localhost:5215
```

### Test API
```bash
# Lihat daftar peminjaman
curl http://localhost:5215/api/peminjaman

# Lihat detail peminjaman
curl http://localhost:5215/api/peminjaman/1

# Ubah status
curl -X PATCH http://localhost:5215/api/peminjaman/1/status \
  -H "Content-Type: application/json" \
  -d '{"status":"Approved"}'
```

---

## Kesimpulan

✅ **Semua requirement telah diimplementasikan:**
1. Pencatatan Peminjaman Ruangan - LENGKAP
2. Pengelolaan Status Peminjaman - LENGKAP
3. Riwayat dan Penelusuran Peminjaman - LENGKAP
4. Entity, DTO, dan Controller CRUD - LENGKAP
5. Migration dan Data Seeding - LENGKAP

Sistem siap untuk production dengan error handling yang baik, validasi input yang ketat, dan separation of concerns yang jelas.
