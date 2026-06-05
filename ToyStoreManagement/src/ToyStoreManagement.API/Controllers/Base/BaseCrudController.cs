using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseCrudController<T> : ControllerBase where T: class
    {
        protected readonly IRepository<T> _repository;

        public BaseCrudController(IRepository<T> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<T>>.SuccessResponse(data));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) 
                return NotFound(ApiResponse<T>.FailureResponse("Resource not found"));
            
            return Ok(ApiResponse<T>.SuccessResponse(item));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(T entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return Ok(ApiResponse<T>.SuccessResponse(entity, "Created successfully"));
        }

        [HttpPost("CreateMultiple")]
        public virtual async Task<IActionResult> Create([FromBody] IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                return BadRequest(ApiResponse<object>.FailureResponse("Danh sách đối tượng không được để trống."));

            await _repository.AddRangeAsync(entities);
            await _repository.SaveChangesAsync();

            return Ok(ApiResponse<IEnumerable<T>>.SuccessResponse(entities, $"Đã chèn thành công {entities.Count()} bản ghi."));
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update(T entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null, "Updated successfully"));
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) 
                return NotFound(ApiResponse<object>.FailureResponse("Resource not found"));

            _repository.Delete(item);
            await _repository.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null, "Deleted successfully"));
        }
    }
}
