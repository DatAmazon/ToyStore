namespace ToyStoreManagement.Domain.Constants
{
    public static class OrderStatuses
    {
        public const string Pending = "Chờ xác nhận";
        public const string Confirmed = "Đã xác nhận";
        public const string Processing = "Đang đóng gói";
        public const string Shipping = "Đang giao hàng";
        public const string Delivered = "Đã giao thành công";
        public const string Cancelled = "Đã hủy";
    }
}
