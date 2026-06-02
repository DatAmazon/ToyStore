namespace ToyStoreManagement.Application.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Xuất danh sách sản phẩm ra tệp Excel sử dụng template
        /// </summary>
        Task<byte[]> ExportProductsToExcelAsync(string templatePath, object? criteria = null);

        /// <summary>
        /// Xuất báo cáo danh sách sản phẩm ra tệp PDF chuyên nghiệp
        /// </summary>
        Task<byte[]> ExportProductsToPdfAsync(string templatePath, string title, string creatorName);
    }
}
