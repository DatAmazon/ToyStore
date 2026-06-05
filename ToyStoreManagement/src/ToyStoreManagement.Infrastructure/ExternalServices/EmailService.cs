using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendOrderConfirmationEmailAsync(string customerName, string orderId)
        {
            // Mô phỏng việc gửi email tốn thời gian
            await Task.Delay(2000); 
            _logger.LogInformation("Đã gửi email xác nhận thành công cho khách hàng {CustomerName} với mã đơn: {OrderId}", customerName, orderId);
        }
    }
}
