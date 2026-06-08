using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Domain.Constants;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/admin/audit-logs")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class AuditLogsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentLogs([FromQuery] int count = 50)
        {
            var logs = await _unitOfWork.Repository<AuditLog>().GetQueryable()
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(logs));
        }
    }
}
