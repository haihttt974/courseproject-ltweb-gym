using gym.Data;
using gym.Models;
using gym.VnPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace gym.Controllers
{
    public class PaymentController : Controller
    {
        private readonly GymContext _context;
        private readonly VnPaySettings _vnPaySettings;

        public PaymentController(GymContext context, IOptions<VnPaySettings> vnPayOptions)
        {
            _context = context;
            _vnPaySettings = vnPayOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return NotFound("Không tìm thấy thanh toán.");
            }

            if (payment.IsPaid)
            {
                return BadRequest("Giao dịch này đã được thanh toán.");
            }

            var vnPay = new VnPayLibrary();
            var amount = (long)(payment.Total * 100);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            vnPay.AddRequestData("vnp_Version", "2.1.0");
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
            vnPay.AddRequestData("vnp_Amount", amount.ToString());
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", ipAddress);
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toán #{payment.PaymentId}");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
            vnPay.AddRequestData("vnp_TxnRef", payment.PaymentId.ToString());

            string paymentUrl = vnPay.CreateRequestUrl(_vnPaySettings.Url, _vnPaySettings.HashSecret);
            return Redirect(paymentUrl);
        }


        public async Task<IActionResult> VnPayReturn()
        {
            var vnpay = new VnPayLibrary();

            // ✅ Đọc tất cả các tham số từ query string
            foreach (var (key, value) in Request.Query)
            {
                if (key.StartsWith("vnp_"))
                    vnpay.AddResponseData(key, value);
            }

            // ✅ Lấy dữ liệu từ VNPay
            var paymentIdStr = vnpay.GetResponseData("vnp_TxnRef");
            var responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var secureHash = Request.Query["vnp_SecureHash"];

            if (!int.TryParse(paymentIdStr, out int paymentId))
            {
                ViewBag.Message = "Mã giao dịch không hợp lệ.";
                return View("PaymentFailed");
            }

            // ✅ Xác thực chữ ký
            var isValid = vnpay.ValidateSignature(secureHash, _vnPaySettings.HashSecret);
            if (!isValid)
            {
                ViewBag.Message = "Chữ ký không hợp lệ.";
                return View("PaymentFailed");
            }

            // ✅ Truy vấn thanh toán từ DB
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                ViewBag.Message = "Không tìm thấy giao dịch thanh toán.";
                return View("PaymentFailed");
            }

            // ✅ Kiểm tra kết quả giao dịch
            if (responseCode == "00")
            {
                if (!payment.IsPaid)
                {
                    payment.IsPaid = true;
                    payment.Note = "Thanh toán thành công qua VNPay.";
                    await _context.SaveChangesAsync();
                }

                TempData["PaymentMethod"] = "VNPAY";
                TempData["Message"] = "Thanh toán thành công!";
                return View("PaymentSuccess", payment);
            }

            // ❌ Thanh toán thất bại
            payment.Note = $"Thanh toán thất bại - mã lỗi: {responseCode}";
            await _context.SaveChangesAsync();

            ViewBag.Message = $"Thanh toán không thành công. Mã lỗi: {responseCode}";
            return View("PaymentFailed");
        }

    }
}
