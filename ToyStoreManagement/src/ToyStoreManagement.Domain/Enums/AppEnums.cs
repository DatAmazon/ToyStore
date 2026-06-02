using System.ComponentModel.DataAnnotations;

namespace ToyStoreManagement.Domain.Enums
{
    public class AppEnums
    {
        // 1. Trạng thái đơn hàng
        public enum OrderStatus
        {
            [Display(Name = "Pending", Description = "Chờ xử lý")]
            Pending = 0,

            [Display(Name = "Confirmed", Description = "Đã xác nhận")]
            Confirmed = 1,

            [Display(Name = "Shipping", Description = "Đang giao hàng")]
            Shipping = 2,

            [Display(Name = "Completed", Description = "Hoàn thành")]
            Completed = 3,

            [Display(Name = "Cancelled", Description = "Đã hủy")]
            Cancelled = 4
        }

        // 2. Phương thức thanh toán
        public enum PaymentMethod
        {
            [Display(Name = "Cash", Description = "Tiền mặt")]
            Cash = 0,

            [Display(Name = "Bank Transfer", Description = "Chuyển khoản ngân hàng")]
            BankTransfer = 1,

            [Display(Name = "Credit Card", Description = "Thẻ tín dụng")]
            CreditCard = 2,

            [Display(Name = "MoMo E-wallet", Description = "Ví điện tử MoMo")]
            MoMo = 3
        }

        // 3. Trạng thái sản phẩm
        public enum ProductStatus
        {
            [Display(Name = "In Stock", Description = "Còn hàng")]
            InStock = 0,

            [Display(Name = "Out of Stock", Description = "Hết hàng")]
            OutOfStock = 1,

            [Display(Name = "Discontinued", Description = "Ngừng kinh doanh")]
            Discontinued = 2
        }

        // 4. Loại giao dịch kho
        public enum InventoryTransactionType
        {
            [Display(Name = "Import", Description = "Nhập kho")]
            Import = 0,
            
            [Display(Name = "Export", Description = "Xuất kho")]
            Export = 1
        }
        // 5. Loại giảm giá
        public enum DiscountType
        {
            [Display(Name = "Percentage", Description = "Phần trăm")]
            Percentage = 0,

            [Display(Name = "Fixed Amount", Description = "Số tiền cố định")]
            FixedAmount = 1
        }
    }
}
