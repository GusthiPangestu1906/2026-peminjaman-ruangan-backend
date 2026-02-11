using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2026_peminjaman_ruangan_backend.Migrations
{
    /// <inheritdoc />
    public partial class RestoreRoomsAndAddPeminjamanData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kembalikan data ruangan yang dihapus di migration sebelumnya
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "IsAvailable", "Location", "Name" },
                values: new object[,]
                {
                    { 1, 30, true, "Gedung D3", "Ruang Teori 1" },
                    { 2, 25, true, "Gedung D4", "Lab Informatika" },
                    { 3, 100, true, "Gedung TC", "Aula Pens" },
                    { 4, 15, false, "Gedung Pusat", "Ruang Rapat" },
                    { 5, 50, true, "Gedung Pasca", "Theater PENS" }
                });

            // Tambahkan beberapa data peminjaman sample
            migrationBuilder.InsertData(
                table: "Peminjamans",
                columns: new[] { "Id", "RoomId", "Peminjam", "TanggalPinjam", "Keperluan", "Status" },
                values: new object[,]
                {
                    { 1, 1, "Gusthi Pangestu", new DateTime(2026, 2, 11, 0, 0, 0, DateTimeKind.Utc), "Rapat Koordinasi Tim", "Approved" },
                    { 2, 2, "Rara Amelia", new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc), "Lab Praktikum Sistem Basis Data", "Pending" },
                    { 3, 3, "Andi Wijaya", new DateTime(2026, 2, 13, 0, 0, 0, DateTimeKind.Utc), "Seminar Teknologi Cloud", "Approved" },
                    { 4, 5, "Siti Aminah", new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc), "Workshop Web Development", "Pending" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Hapus data peminjaman
            migrationBuilder.DeleteData(
                table: "Peminjamans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Peminjamans",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Peminjamans",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Peminjamans",
                keyColumn: "Id",
                keyValue: 4);

            // Hapus data ruangan
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
