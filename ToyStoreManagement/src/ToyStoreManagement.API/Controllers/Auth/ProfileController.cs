using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(Guid.Parse(GetUserId()));
            if (customer == null) return NotFound(ApiResponse<object>.FailureResponse("Không tìm thấy thông tin người dùng."));

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                customer.FullName,
                customer.Email,
                customer.Phone,
                customer.Address
            }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] Customer updateData)
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(Guid.Parse(GetUserId()));
            if (customer == null) return NotFound(ApiResponse<object>.FailureResponse("Không tìm thấy thông tin người dùng."));

            customer.FullName = updateData.FullName;
            customer.Phone = updateData.Phone;
            customer.Address = updateData.Address;

            _unitOfWork.Repository<Customer>().Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật hồ sơ thành công."));
        }
    }
}
