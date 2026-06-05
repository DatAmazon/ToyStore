using System.Threading.Tasks;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(string customerName, string orderId);
    }
}
