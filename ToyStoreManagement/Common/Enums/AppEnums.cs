using System.ComponentModel.DataAnnotations;

namespace ToyStoreManagement.Common.Enums
{
    public class AppEnums
    {
        // 1. Trạng thái đơn hàng
        public enum OrderStatus
        {
            [Display(Name = "Chờ xử lý")]
            Pending = 0,

            [Display(Name = "Đã xác nhận")]
            Confirmed = 1,

            [Display(Name = "Đang giao hàng")]
            Shipping = 2,

            [Display(Name = "Hoàn thành")]
            Completed = 3,

            [Display(Name = "Đã hủy")]
            Cancelled = 4
        }

        // 2. Phương thức thanh toán
        public enum PaymentMethod
        {
            [Display(Name = "Tiền mặt")]
            Cash = 0,

            [Display(Name = "Chuyển khoản ngân hàng")]
            BankTransfer = 1,

            [Display(Name = "Thẻ tín dụng")]
            CreditCard = 2,

            [Display(Name = "Ví điện tử MoMo")]
            MoMo = 3
        }

        // 3. Trạng thái sản phẩm
        public enum ProductStatus
        {
            [Display(Name = "Còn hàng")]
            InStock = 0,

            [Display(Name = "Hết hàng")]
            OutOfStock = 1,

            [Display(Name = "Ngừng kinh doanh")]
            Discontinued = 2
        }

        // 4. Loại giao dịch kho
        public enum InventoryTransactionType
        {
            [Display(Name = "Nhập kho")]
            Import = 0, // Nhập kho
            [Display(Name = "Xuất kho")]
            Export = 1  // Xuất kho
        }
    }

}
