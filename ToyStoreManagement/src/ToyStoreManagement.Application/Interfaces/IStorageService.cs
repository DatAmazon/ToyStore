using System.IO;
using System.Threading.Tasks;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "");
        Task<bool> DeleteFileAsync(string fileKey, string folder = "");
        string GetFileUrl(string fileKey, string folder = "");
    }
}
