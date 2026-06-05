using System.IO;
using System.Threading.Tasks;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "toystore");
        Task<bool> DeleteFileAsync(string fileKey, string folder = "toystore");
        string GetFileUrl(string fileKey, string folder = "toystore");
    }
}
