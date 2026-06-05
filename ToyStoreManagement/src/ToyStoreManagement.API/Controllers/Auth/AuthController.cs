using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs.Auth;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.Success 
                ? Ok(ApiResponse<object>.SuccessResponse(null, result.Message)) 
                : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result.Success 
                ? Ok(ApiResponse<object>.SuccessResponse(result.Data)) 
                : Unauthorized(ApiResponse<object>.FailureResponse(result.Message));
        }
    }
}
