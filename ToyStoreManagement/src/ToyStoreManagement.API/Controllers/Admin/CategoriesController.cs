using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces.IAdminService;
using ToyStoreManagement.Application.Services.AdminService;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseCrudController<Category>
    {
        private readonly ICategoriesService _categoriesService;
        private readonly IMapper _mapper;

        public CategoriesController(IRepository<Category> repository, ICategoriesService categoriesService, IMapper mapper) : base(repository)
        {
            _categoriesService = categoriesService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public override async Task<IActionResult> GetAll()
        {
            var categories = await _categoriesService.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos));
        }

        [HttpPost("Create")]
        public override async Task<IActionResult> Create([FromBody] IEnumerable<Category> categories)
        {
            var result = await _categoriesService.CreateMultipleAsync(categories);
            if (!result.Success) return BadRequest(ApiResponse<object>.FailureResponse(result.Message));
            return Ok(ApiResponse<object>.SuccessResponse(null, result.Message));
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto category)
        {
            await _categoriesService.UpdateAsync(category);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var success = await _categoriesService.DeleteAsync(id);
            if (!success) throw new KeyNotFoundException($"Không tìm thấy danh mục với mã ID: {id}");
            return Ok(ApiResponse<object>.SuccessResponse(null, "Deleted successfully"));
        }

        // Ẩn các phương thức base để tránh Swagger 500
        [NonAction]
        public override Task<IActionResult> Update(Category entity) => base.Update(entity);
        [NonAction]
        public override Task<IActionResult> Delete(Guid id) => base.Delete(id);
        [NonAction]
        public override Task<IActionResult> Create(Category entity) => base.Create(entity);
    }
}
