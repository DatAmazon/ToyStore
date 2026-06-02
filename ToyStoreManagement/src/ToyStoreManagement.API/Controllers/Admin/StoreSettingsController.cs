using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreSettingsController : BaseCrudController<StoreSetting>
    {
        public StoreSettingsController(IRepository<StoreSetting> repository) : base(repository) { }
    }
}
