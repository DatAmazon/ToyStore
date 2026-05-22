using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.IRepositories;

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
            => Ok(await _repository.GetAllAsync());

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(T entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update(T entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();
            _repository.Delete(item);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}
