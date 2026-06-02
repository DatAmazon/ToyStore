using System.Threading.Tasks;

namespace ToyStoreManagement.Application.Interfaces.IAdminService
{
    public interface IProductService
    {
        Task<byte[]> ExportProductsToExcelAsync();
    }
}
