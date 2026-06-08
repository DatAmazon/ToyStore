using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToyStoreManagement.Application.DTOs.Auth;
using ToyStoreManagement.Application.Interfaces;
using System.Linq;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;
using ToyStoreManagement.Domain.Constants;

namespace ToyStoreManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, 
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác.");

            var token = await GenerateJwtToken(user); 
            return (true, "Login successful.", new AuthResponseDto(token, user.UserName!));
        }

        public async Task<(bool Success, string Message, AuthResponseDto? Data)> GoogleLoginAsync(GoogleLoginDto dto)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"]! }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
                
                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        EmailConfirmed = true
                    };
                    
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        return (false, $"Không thể tạo tài khoản: {errors}", null);
                    }

                    // Gán quyền mặc định là Customer
                    await _userManager.AddToRoleAsync(user, AppRoles.Customer);

                    // Tạo Customer tương ứng
                    var customer = new Customer
                    {
                        CustomerId = Guid.Parse(user.Id), // Đồng nhất ID giữa Identity và Customer
                        Email = payload.Email,
                        FullName = payload.Name ?? payload.Email,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.Repository<Customer>().AddAsync(customer);
                    await _unitOfWork.SaveChangesAsync();
                }

                var token = await GenerateJwtToken(user);
                return (true, "Google login successful.", new AuthResponseDto(token, user.UserName!));
            }
            catch (InvalidJwtException ex)
            {
                return (false, $"Token Google không hợp lệ: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi đăng nhập Google: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            var user = new IdentityUser { Email = dto.Email, UserName = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(x => x.Description));

                throw new ArgumentException(errors);
            }

            // Gán quyền mặc định là Customer
            await _userManager.AddToRoleAsync(user, AppRoles.Customer);

            // Tạo Customer tương ứng
            var customer = new Customer
            {
                CustomerId = Guid.Parse(user.Id), // Đồng nhất ID giữa Identity và Customer
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.Now
            };
            await _unitOfWork.Repository<Customer>().AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Registration successful.");
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
