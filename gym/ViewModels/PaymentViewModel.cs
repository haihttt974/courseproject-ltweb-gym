namespace gym.ViewModels
{
    public class PaymentViewModel
    {
        public int MemberId { get; set; }
        public int PackageId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Tiền mặt"; // hoặc "Chuyển khoản", etc.
    }
}
