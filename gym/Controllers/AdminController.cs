using Microsoft.AspNetCore.Mvc;
using gym.Models; // Đổi thành namespace chứa DbContext & entity
using gym.ViewModels; // Sẽ tạo ở bước 2
using System;
using System.Linq;
using gym.Data;

public class AdminController : Controller
{
    private readonly GymContext _context;

    public AdminController(GymContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var totalMembers = _context.Members.Count();

        var currentDate = DateTime.Now;
        var currentMonth = currentDate.Month;
        var currentYear = currentDate.Year;

        // Lấy tổng doanh thu tháng này
        var revenueThisMonth = (
            from mp in _context.MemberPayments
            join p in _context.Payments on mp.PaymentId equals p.PaymentId
            where mp.PaymentDate.HasValue &&
                  mp.PaymentDate.Value.Month == currentMonth &&
                  mp.PaymentDate.Value.Year == currentYear
            select p.Total
        ).Sum();


        // Hội viên có gói tập sắp hết hạn
        var expiringSoonCount = _context.MemberPakages
               .Where(p => p.EndDate.HasValue &&
                           p.EndDate.Value >= currentDate &&
                           p.EndDate.Value <= currentDate.AddDays(7))
               .Count();

        // Thông báo mới nhất
        var latestNotifications = _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToList();

        var viewModel = new AdminDashboardViewModel
        {
            TotalMembers = totalMembers,
            RevenueThisMonth = revenueThisMonth,
            ExpiringSoonCount = expiringSoonCount,
            LatestNotifications = latestNotifications
        };

        return View(viewModel);
    }
}
