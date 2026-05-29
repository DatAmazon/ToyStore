using System.Threading.Tasks;

namespace ToyStoreManagement.IServices.IAdminService
{
    public interface IProductService
    {
        Task<byte[]> ExportProductsToExcelAsync();
    }
}
