using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    // Thông tin User
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    // Thông tin Member
    [Required]
    public string FullName { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public bool Sex { get; set; } // true = nam, false = nữ

    [Required]
    public string Phone { get; set; }

    [Required]
    public string Address { get; set; }
}
