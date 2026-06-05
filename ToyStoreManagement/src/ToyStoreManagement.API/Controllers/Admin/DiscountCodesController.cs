using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCodesController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly IRepository<DiscountCode> _repository;

        public DiscountCodesController(IDiscountService discountService, IRepository<DiscountCode> repository)
        {
            _discountService = discountService;
            _repository = repository;
        }

        [HttpGet("validate/{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            var result = await _discountService.ValidateDiscountAsync(code);
            return result.Success 
                ? Ok(ApiResponse<DiscountCode>.SuccessResponse(result.Discount!)) 
                : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<DiscountCode>>.SuccessResponse(data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiscountCode discount)
        {
            await _repository.AddAsync(discount);
            await _repository.SaveChangesAsync();
            return Ok(ApiResponse<DiscountCode>.SuccessResponse(discount, "Created successfully"));
        }
    }
}
