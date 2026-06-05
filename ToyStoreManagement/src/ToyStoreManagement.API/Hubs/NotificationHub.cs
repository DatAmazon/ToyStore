using Microsoft.AspNetCore.SignalR;

namespace ToyStoreManagement.API.Hubs
{
    public class NotificationHub : Hub
    {
        // Hub này sẽ được gọi từ phía Server để đẩy thông báo xuống Client (Admin)
        // Client (ReactJS) sẽ connect tới đây qua route "/hubs/notification"
        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
