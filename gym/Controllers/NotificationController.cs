using gym.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gym.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly GymContext _context;

        public NotificationController(GymContext context)
        {
            _context = context;
        }

        // ✅ Hiển thị danh sách thông báo của người dùng
        public async Task<IActionResult> List()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Unauthorized();

            var notifications = await _context.UserNotifications
                .Include(un => un.Notification)
                .Where(un => un.UserId == user.UserId)
                .OrderByDescending(un => un.TimeSend)
                .ToListAsync();

            return View(notifications);
        }

        // ✅ API: Trả về số lượng thông báo chưa đọc (dành cho nút chuông)
        [HttpGet]
        public async Task<IActionResult> UnseenCount()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Json(0);

            var count = await _context.UserNotifications
                .Where(n => n.UserId == user.UserId && n.Seen == false)
                .CountAsync();

            return Json(count);
        }

        // ✅ AJAX: Xem chi tiết + đánh dấu là đã đọc (dùng trong modal)
        [HttpPost]
        public async Task<IActionResult> MarkAsSeen(int id)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Unauthorized();

            var userNotification = await _context.UserNotifications
                .Include(un => un.Notification)
                .FirstOrDefaultAsync(un => un.UserId == user.UserId && un.NotificationId == id);

            if (userNotification == null) return NotFound();

            userNotification.Seen = true;
            await _context.SaveChangesAsync();

            return PartialView("_NotificationDetailPartial", userNotification.Notification);
        }
    }
}
