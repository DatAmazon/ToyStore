using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IRepository<DiscountCode> _discountRepo;

        public DiscountService(IRepository<DiscountCode> discountRepo)
        {
            _discountRepo = discountRepo;
        }

        public async Task<DiscountCode?> GetByCodeAsync(string code)
        {
            return await _discountRepo.GetQueryable()
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<(bool Success, string Message, DiscountCode? Discount)> ValidateDiscountAsync(string code)
        {
            var discount = await GetByCodeAsync(code);
            
            if (discount == null)
                return (false, "Discount code not found.", null);

            if (DateTime.UtcNow < discount.StartDate || DateTime.UtcNow > discount.EndDate)
                return (false, "Discount code is expired or not yet active.", null);

            if (discount.UsageLimit > 0 && discount.UsedCount >= discount.UsageLimit)
                return (false, "Discount code usage limit reached.", null);

            return (true, "Discount code is valid.", discount);
        }

        public async Task<bool> ApplyDiscountAsync(Guid discountId)
        {
            var discount = await _discountRepo.GetByIdAsync(discountId);
            if (discount == null) return false;

            discount.UsedCount++;
            await _discountRepo.SaveChangesAsync();
            return true;
        }
    }
}
