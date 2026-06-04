namespace ToyStoreManagement.Application.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Xuất danh sách sản phẩm ra tệp Excel sử dụng template mặc định
        /// </summary>
        Task<byte[]> GetProductExcelReportAsync();

        /// <summary>
        /// Xuất báo cáo danh sách sản phẩm ra tệp PDF chuyên nghiệp
        /// </summary>
        Task<byte[]> GetProductPdfReportAsync(string title, string creatorName);
    }
}
