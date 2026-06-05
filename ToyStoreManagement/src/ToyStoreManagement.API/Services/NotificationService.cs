using Microsoft.AspNetCore.SignalR;
using ToyStoreManagement.API.Hubs;
using ToyStoreManagement.Application.Interfaces;
using System.Threading.Tasks;

namespace ToyStoreManagement.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
