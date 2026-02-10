using System.ComponentModel.DataAnnotations;

namespace _2026_peminjaman_ruangan_backend.Models;

public class CustomerDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}