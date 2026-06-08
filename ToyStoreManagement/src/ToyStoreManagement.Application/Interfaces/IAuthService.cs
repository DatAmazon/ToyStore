using System.Threading.Tasks;
using ToyStoreManagement.Application.DTOs.Auth;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginDto dto);
        Task<(bool Success, string Message, AuthResponseDto? Data)> GoogleLoginAsync(GoogleLoginDto dto);
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
    }
}
