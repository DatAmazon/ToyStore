using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Data;
using ToyStoreManagement.Entities;
using ToyStoreManagement.IRepositories;
using ToyStoreManagement.Services.AdminService;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseCrudController<Category>
    {
        private readonly CategoriesService _categoriesService;

        public CategoriesController(IRepository<Category> repository, CategoriesService categoriesService)
            : base(repository)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("GetAll")]
        public override async Task<IActionResult> GetAll()
        {
            var result = await _categoriesService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("Create")]
        public override async Task<IActionResult> Create([FromBody] IEnumerable<Category> categories)
        {
            try
            {
                var result = await _categoriesService.CreateMultipleAsync(categories);

                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(new { message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi Oracle: {ex.Message}");
            }
        }
    }
}
