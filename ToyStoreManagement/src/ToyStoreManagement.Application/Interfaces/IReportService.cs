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

        /// <summary>
        /// Xuất danh sách đơn hàng ra tệp Excel
        /// </summary>
        Task<byte[]> GetOrderExcelReportAsync(DateTime? fromDate, DateTime? toDate);

        /// <summary>
        /// Xuất hóa đơn chi tiết ra tệp PDF
        /// </summary>
        Task<byte[]> GetInvoicePdfAsync(Guid orderId);

        /// <summary>
        /// Xuất báo cáo doanh thu theo khoảng thời gian
        /// </summary>
        Task<byte[]> GetSalesReportExcelAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Xuất danh sách khách hàng ra Excel
        /// </summary>
        Task<byte[]> GetCustomerReportExcelAsync();
    }
}
