using Azure.Core;
using gym.Data;
using gym.Models.Vnpay;
using gym.Services.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace gym.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly GymContext _context;
        public PaymentController(IVnPayService vnPayService, GymContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
            //TempData["VNPayUrl"] = url;
            //return RedirectToAction("DebugUrl");
        }
        public IActionResult DebugUrl()
        {
            ViewBag.Url = TempData["VNPayUrl"]?.ToString();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success)
            {
                TempData["Error"] = "Thanh toán không hợp lệ hoặc bị hủy.";
                return RedirectToAction("Unpaid", "Booking");
            }

            // Trích xuất PaymentId từ OrderDescription hoặc Name
            var match = Regex.Match(response.OrderDescription, @"PaymentId[:\- ]?(\d+)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                TempData["Error"] = "Không thể xác định được giao dịch cần xử lý.";
                return RedirectToAction("Unpaid", "Booking");
            }

            int paymentId = int.Parse(match.Groups[1].Value);

            // Tìm và cập nhật
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
            {
                TempData["Error"] = "Không tìm thấy thanh toán.";
                return RedirectToAction("Unpaid", "Booking");
            }

            payment.IsPaid = true;

            var memberPayment = await _context.MemberPayments
                .FirstOrDefaultAsync(mp => mp.PaymentId == paymentId);

            if (memberPayment != null)
                memberPayment.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Thanh toán thành công!";
            return RedirectToAction("Unpaid", "Booking");
        }


    }
}
