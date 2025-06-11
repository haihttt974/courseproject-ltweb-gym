using gym.Data;
using gym.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly GymContext _context;

    public AdminController(GymContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalMembers = await _context.Members.CountAsync(),
            TotalActivePackages = await _context.MemberPakages.CountAsync(mp => mp.IsActive == true),
            TotalRevenue = await _context.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0
        };

        model.PackageStats = await _context.Packages
            .Select(pkg => new PackageStat
            {
                PackageName = pkg.Name,
                MemberCount = _context.MemberPakages
                    .Count(mp => mp.PackageId == pkg.PackageId && mp.IsActive == true),

                TotalRevenue = _context.MemberPakages
                    .Where(mp => mp.PackageId == pkg.PackageId && mp.IsPaid == true)
                    .Join(_context.MemberPayments,
                          mp => new { mp.MemberId },
                          pay => new { pay.MemberId },
                          (mp, pay) => pay)
                    .Join(_context.Payments,
                          pay => pay.PaymentId,
                          p => p.PaymentId,
                          (pay, p) => p.Amount)
                    .Sum().GetValueOrDefault()
            })
            .ToListAsync();


        return View(model);
    }
}
