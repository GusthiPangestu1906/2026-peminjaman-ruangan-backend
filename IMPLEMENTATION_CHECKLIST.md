# Checklist Implementasi - Sistem Peminjaman Ruangan

## ✅ REQUIREMENT 1: Pencatatan Peminjaman Ruangan

### Menambah Data Peminjaman
- ✅ **Method**: POST `/api/peminjaman`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L48)
- ✅ **DTO**: [PeminjamanDto.cs](Models/PeminjamanDto.cs)
- ✅ **Validasi**:
  - Nama peminjam tidak kosong
  - RoomId valid (> 0)
  - Tanggal tidak masa lalu
  - Cek booking conflict
  - **Limit 2 booking per user/hari**
- ✅ **Response**: 201 Created dengan data peminjaman + FK Room detail

### Melihat Daftar Peminjaman
- ✅ **Method**: GET `/api/peminjaman`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L17)
- ✅ **Fitur**:
  - Query parameter `?search=` untuk pencarian
  - Include Room detail (eager loading)
  - Case-insensitive search pada nama peminjam & keperluan
- ✅ **Response**: 200 OK dengan array data

### Melihat Detail Peminjaman
- ✅ **Method**: GET `/api/peminjaman/{id}`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L170)
- ✅ **Fitur**:
  - Tampilkan detail peminjaman spesifik
  - Include Room data
- ✅ **Response**: 200 OK dengan single object

### Mengubah Data Peminjaman
- ✅ **Method**: PUT `/api/peminjaman/{id}`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L212)
- ✅ **DTO**: [PeminjamanDto.cs](Models/PeminjamanDto.cs)
- ✅ **Validasi**:
  - Tanggal tidak masa lalu
  - Cek booking conflict (abaikan diri sendiri)
- ✅ **Response**: 204 No Content

### Menghapus Data Peminjaman
- ✅ **Method**: DELETE `/api/peminjaman/{id}`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L260)
- ✅ **Response**: 204 No Content

---

## ✅ REQUIREMENT 2: Pengelolaan Status Peminjaman

### Melihat Status Peminjaman
- ✅ **Implementasi**: Bagian dari PeminjamanDetailDto
- ✅ **File**: [PeminjamanDetailDto.cs](Models/PeminjamanDetailDto.cs)
- ✅ **Di-include dalam**: 
  - GET `/api/peminjaman` (list)
  - GET `/api/peminjaman/{id}` (detail)
  - GET `/api/peminjaman/status/{status}` (filter)

### Mengubah Status Peminjaman
- ✅ **Method**: PATCH `/api/peminjaman/{id}/status`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L126)
- ✅ **DTO**: [StatusUpdateDto.cs](Models/StatusUpdateDto.cs)
- ✅ **Status yang Valid**:
  - Pending (default)
  - Approved
  - Rejected
- ✅ **Validasi**: Status harus salah satu dari list di atas
- ✅ **Response**: 204 No Content

### Menyimpan Riwayat Perubahan Status
- ℹ️ **Status**: Opsional (belum diimplementasikan)
- 💡 **Catatan**: Dapat ditambahkan nanti dengan menambahkan:
  - Tabel `PeminjamanStatusHistory` (Entity)
  - Trigger pada saat PATCH status
  - Menyimpan: PeminjamanId, StatusLama, StatusBaru, WaktuPerubahan, DiubahOleh

---

## ✅ REQUIREMENT 3: Riwayat dan Penelusuran Peminjaman

### Menampilkan Riwayat Peminjaman
- ✅ **Method**: GET `/api/peminjaman`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L17)
- ✅ **Fitur**: Menampilkan semua peminjaman dengan data lengkap

### Pencarian Data Peminjaman
- ✅ **Method**: GET `/api/peminjaman?search={query}`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L22-29)
- ✅ **Query**: 
  - Pencarian pada field `Peminjam` (nama peminjam)
  - Pencarian pada field `Keperluan`
  - Case-insensitive
- ✅ **Response**: 200 OK dengan hasil filter

### Filter Data Berdasarkan Status
- ✅ **Method**: GET `/api/peminjaman/status/{status}`
- ✅ **File**: [PeminjamanController.cs](Controllers/PeminjamanController.cs#L158)
- ✅ **Status Filter**: Pending | Approved | Rejected
- ✅ **Response**: 200 OK atau 404 Not Found

### Pengurutan Data (Opsional)
- ℹ️ **Status**: Belum diimplementasikan
- 💡 **Saran Penambahan**: 
  - Query parameter: `?orderBy=tanggalPinjam&order=asc/desc`
  - atau: `?sortBy=peminjam&sort=desc`

---

## ✅ REQUIREMENT 4: Entity, DTO, dan Controller CRUD

### Entity Classes
- ✅ **Peminjaman.cs** - [Models/Peminjaman.cs](Models/Peminjaman.cs)
  - Id (Primary Key)
  - RoomId (Foreign Key)
  - Room (Navigation Property)
  - Peminjam (Required)
  - TanggalPinjam (Required)
  - Keperluan
  - Status (Default: "Pending")

- ✅ **Room.cs** - [Models/Room.cs](Models/Room.cs)
  - Id (Primary Key)
  - Name
  - Capacity
  - Location
  - IsAvailable

- ✅ **Customer.cs** - [Models/Customer.cs](Models/Customer.cs)
  - Id (Primary Key)
  - Name (Required)
  - Email (Required, dengan EmailAddress attribute)
  - Phone

### DTO Classes

#### Request DTOs
- ✅ **PeminjamanDto.cs** - [Models/PeminjamanDto.cs](Models/PeminjamanDto.cs)
  - Digunakan untuk: POST dan PUT requests
  - Fields: RoomId, Peminjam, TanggalPinjam, Keperluan
  - Data Annotations: Required

- ✅ **StatusUpdateDto.cs** - [Models/StatusUpdateDto.cs](Models/StatusUpdateDto.cs)
  - Digunakan untuk: PATCH requests
  - Field: Status

#### Response DTOs
- ✅ **PeminjamanDetailDto.cs** - [Models/PeminjamanDetailDto.cs](Models/PeminjamanDetailDto.cs)
  - Digunakan untuk: GET responses
  - Includes: RoomDto (nested object)
  - Fields: Id, RoomId, Room, Peminjam, TanggalPinjam, Status, Keperluan

- ✅ **RoomDto.cs** - [Models/RoomDto.cs](Models/RoomDto.cs)
  - Digunakan untuk: Room data dalam response
  - Fields: Id, Name, Capacity, Location

- ✅ **CustomerDto.cs** - [Models/CustomerDto.cs](Models/CustomerDto.cs)
  - Untuk: Customer responses

### Controllers

- ✅ **PeminjamanController.cs** - [Controllers/PeminjamanController.cs](Controllers/PeminjamanController.cs)
  - GET `/api/peminjaman` (List dengan search)
  - GET `/api/peminjaman/{id}` (Detail)
  - POST `/api/peminjaman` (Create)
  - PUT `/api/peminjaman/{id}` (Update)
  - PATCH `/api/peminjaman/{id}/status` (Update Status)
  - DELETE `/api/peminjaman/{id}` (Delete)
  - GET `/api/peminjaman/status/{status}` (Filter by Status)

- ✅ **RoomController.cs** - [Controllers/RoomController.cs](Controllers/RoomController.cs)
  - GET `/api/room` (List all rooms)
  - GET `/api/room/available` (Check availability)
  - GET `/api/room/{id}/bookings` (Check schedule)

- ✅ **CustomerController.cs** - [Controllers/CustomerController.cs](Controllers/CustomerController.cs)
  - Customer management endpoints

### Design Pattern & Principles
- ✅ **Separation of Concerns**:
  - Entity models terpisah dari DTOs
  - Controller hanya handle HTTP routing
  - Business logic di dalam controller methods

- ✅ **Repository Pattern**:
  - Menggunakan DbContext untuk data access
  - Async/await untuk non-blocking operations
  - Include() untuk eager loading relationships

- ✅ **Validation**:
  - Input validation di controller
  - Data annotations di models
  - **Business Logic**: Limit booking & Status-based collision
  - Custom business logic validation

- ✅ **Error Handling**:
  - Return appropriate HTTP status codes
  - Meaningful error messages
  - Try-catch untuk exception handling

---

## ✅ REQUIREMENT 5: Migration dan Data Seeding

### Database Context
- ✅ **AppDbContext.cs** - [Data/AppDbContext.cs](Data/AppDbContext.cs)
  - DbSet<Room>
  - DbSet<Peminjaman>
  - DbSet<Customer>
  - OnModelCreating dengan data seeding untuk Customer

### Migrations
- ✅ **20260206024931_InitialCreate**
  - Create tables: Rooms, Peminjamans, AppDbContext
  
- ✅ **20260206030628_SeedRoomsData**
  - Insert 5 ruangan sample
  
- ✅ **20260206123808_AddPeminjamanTable**
  - Create Peminjaman table dengan FK ke Rooms
  
- ✅ **20260209022307_UpdatePeminjamanModel**
  - Add Keperluan dan Status columns
  
- ✅ **20260209114022_AddCustomerAndSeeding**
  - Create Customers table
  - Insert 2 customer sample
  - (Delete 5 ruangan - diperbaiki di migration berikutnya)
  
- ✅ **20260210060520_RestoreRoomsAndAddPeminjamanData**
  - Restore 5 ruangan
  - Insert 4 peminjaman sample dengan berbagai status
  - Dengan DateTime.Kind.Utc (PostgreSQL compliance)

### Data Seeding
- ✅ **5 Ruangan**:
  ```
  1. Ruang Teori 1 (30 capacity, Gedung D3)
  2. Lab Informatika (25 capacity, Gedung D4)
  3. Aula Pens (100 capacity, Gedung TC)
  4. Ruang Rapat (15 capacity, Gedung Pusat, IsAvailable=false)
  5. Theater PENS (50 capacity, Gedung Pasca)
  ```

- ✅ **4 Peminjaman Sample**:
  ```
  1. Gusthi Pangestu | Ruang Teori 1 | 2026-02-11 | Approved | Rapat Koordinasi Tim
  2. Rara Amelia | Lab Informatika | 2026-02-12 | Pending | Lab Praktikum Sistem Basis Data
  3. Andi Wijaya | Aula Pens | 2026-02-13 | Approved | Seminar Teknologi Cloud
  4. Siti Aminah | Theater PENS | 2026-02-14 | Pending | Workshop Web Development
  ```

- ✅ **2 Customer Sample**:
  ```
  1. Budi Santoso | budi@example.com | 081234567890
  2. Siti Aminah | siti@example.com | 089876543210
  ```

---

## 📊 Summary Status

| No | Requirement | Status | File | Catatan |
|---|---|---|---|---|
| 1.1 | Menambah Peminjaman | ✅ | PeminjamanController.cs:L48 | POST endpoint dengan validasi |
| 1.2 | Melihat Daftar Peminjaman | ✅ | PeminjamanController.cs:L17 | GET dengan search parameter |
| 1.3 | Melihat Detail Peminjaman | ✅ | PeminjamanController.cs:L170 | GET /{id} endpoint |
| 1.4 | Mengubah Data Peminjaman | ✅ | PeminjamanController.cs:L212 | PUT endpoint | 
| 1.5 | Menghapus Data Peminjaman | ✅ | PeminjamanController.cs:L260 | DELETE endpoint |
| 2.1 | Melihat Status Peminjaman | ✅ | PeminjamanDetailDto.cs | Bagian dari response |
| 2.2 | Mengubah Status Peminjaman | ✅ | PeminjamanController.cs:L126 | PATCH endpoint |
| 2.3 | Riwayat Perubahan Status | ℹ️ | - | Opsional, belum diimplementasikan |
| 3.1 | Menampilkan Riwayat | ✅ | PeminjamanController.cs:L17 | GET /api/peminjaman |
| 3.2 | Pencarian Data | ✅ | PeminjamanController.cs:L22 | Query param ?search= |
| 3.3 | Filter Berdasarkan Status | ✅ | PeminjamanController.cs:L158 | GET /status/{status} |
| 3.4 | Pengurutan Data | ℹ️ | - | Opsional, belum diimplementasikan |
| 4.1 | Entity Classes | ✅ | Models/ | Peminjaman, Room, Customer |
| 4.2 | DTO Classes | ✅ | Models/ | 5 DTO tersedia |
| 4.3 | Controllers | ✅ | Controllers/ | PeminjamanController, RoomController |
| 5.1 | Migration | ✅ | Migrations/ | 6 migrations sudah dibuat |
| 5.2 | Data Seeding | ✅ | Migrations/ | 5 ruangan + 4 peminjaman + 2 customer |
| 6.1 | Cek Ketersediaan | ✅ | RoomController.cs:L40 | GET /api/room/available |

---

## 🎯 Fitur Tambahan (Belum Ada)

Untuk melengkapi sistem, berikut adalah saran fitur tambahan:

### 1. Riwayat Perubahan Status
```csharp
// Tambahkan Entity
public class PeminjamanStatusHistory
{
    public int Id { get; set; }
    public int PeminjamanId { get; set; }
    public string StatusLama { get; set; }
    public string StatusBaru { get; set; }
    public DateTime WaktuPerubahan { get; set; }
    public string DiubahOleh { get; set; }
    public Peminjaman? Peminjaman { get; set; }
}

// Trigger pada PATCH status untuk log setiap perubahan
```

### 2. Pengurutan Data
```
GET /api/peminjaman?orderBy=tanggalPinjam&order=asc
GET /api/peminjaman?sort=peminjam
```

### 3. Pagination
```
GET /api/peminjaman?page=1&pageSize=10
```

### 4. Advanced Filtering
```
GET /api/peminjaman?status=Pending&roomId=1&dateFrom=2026-02-10&dateTo=2026-02-20
```

### 5. Validation Error Response Format
```json
{
  "errors": {
    "peminjam": ["Nama peminjam wajib diisi"],
    "tanggalPinjam": ["Tanggal tidak boleh di masa lalu"]
  }
}
```

---

## 🚀 Testing Checklist

### Unit Testing (Recommended)
- [ ] Test POST validation
- [ ] Test booking conflict detection
- [ ] Test search functionality
- [ ] Test status change validation

### Integration Testing (Recommended)
- [ ] Test API endpoints dengan database
- [ ] Test FK relationship
- [ ] Test concurrent requests

### Manual Testing (Done)
- ✅ GET /api/peminjaman
- ✅ GET /api/peminjaman/{id}
- ✅ POST /api/peminjaman
- ✅ PUT /api/peminjaman/{id}
- ✅ PATCH /api/peminjaman/{id}/status
- ✅ DELETE /api/peminjaman/{id}
- ✅ GET /api/peminjaman/status/{status}
