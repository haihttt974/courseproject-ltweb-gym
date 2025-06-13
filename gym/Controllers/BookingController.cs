using gym.Data;
using gym.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Member")]
public class BookingController : Controller
{
    private readonly GymContext _context;

    public BookingController(GymContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Unpaid()
    {
        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.RoleId == 2);
        if (user == null) return Unauthorized();

        int memberId = (int)user.ReferenceId;

        var unpaidPackages = await _context.MemberPayments
            .Include(mp => mp.Payment)
            .Include(mp => mp.Member)
            .Where(mp => mp.MemberId == memberId && mp.Payment != null && !mp.Payment.IsPaid)
            .Select(mp => new UnpaidPackageViewModel
            {
                PaymentId = mp.PaymentId,
                Total = mp.Payment!.Total,
                DueDate = mp.Payment.DueDate,
                Description = mp.Payment.Description,
                IsPaid = mp.Payment.IsPaid
            })
            .ToListAsync();

        return View(unpaidPackages);
    }

    // Điều hướng đến API thanh toán VNPay
    [HttpPost]
    public IActionResult PayWithVNPay(int paymentId, decimal amount)
    {
        // Redirect sang trang VNPay – bạn đã tích hợp sẵn
        string vnpayUrl = $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_TxnRef={paymentId}&vnp_Amount={(long)(amount * 100)}";
        return Redirect(vnpayUrl);
    }
}
