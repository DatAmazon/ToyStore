namespace ToyStoreManagement.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message);
    }
}
