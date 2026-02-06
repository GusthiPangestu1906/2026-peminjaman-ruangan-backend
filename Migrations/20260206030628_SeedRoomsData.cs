using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2026_peminjaman_ruangan_backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoomsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
