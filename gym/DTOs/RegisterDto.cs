using System.ComponentModel.DataAnnotations;

namespace gym.Models.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }
}