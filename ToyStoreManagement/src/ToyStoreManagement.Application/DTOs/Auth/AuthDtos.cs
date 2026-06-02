using System.ComponentModel.DataAnnotations;

namespace ToyStoreManagement.Application.DTOs.Auth
{
    public record LoginDto(
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        string Email,

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6 ký tự")]
        string Password);

    public record RegisterDto(
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        string email,

        [Required(ErrorMessage = "Username là bắt buộc")]
        string UserName,    

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6 ký tự")]
        string Password
    );

    public record AuthResponseDto(string Token, string UserName);
}
