using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs;
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
            return Ok(categoryDtos);
        }

        [HttpPost("Create")]
        public override async Task<IActionResult> Create([FromBody] IEnumerable<Category> categories)
        {
            var result = await _categoriesService.CreateMultipleAsync(categories);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message, data = result.Data });
        }
    }
}
