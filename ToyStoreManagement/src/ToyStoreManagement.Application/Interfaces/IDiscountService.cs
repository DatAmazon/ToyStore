using System.Threading.Tasks;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IDiscountService
    {
        Task<(bool Success, string Message, DiscountCode? Discount)> ValidateDiscountAsync(string code);
        Task<bool> ApplyDiscountAsync(Guid discountId);
        Task<DiscountCode?> GetByCodeAsync(string code);
    }
}
