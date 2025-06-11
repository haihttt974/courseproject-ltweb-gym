using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gym.Data;
using gym.Models; // giả sử model nằm ở đây

namespace gym.Controllers
{
    public class TrainersController : Controller
    {
        private readonly GymContext _context;

        public TrainersController(GymContext context)
        {
            _context = context;
        }

        // -------------------- CRUD mặc định --------------------

        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainerId,FullName,Phone,Specialty,ScheduleNote")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,FullName,Phone,Specialty,ScheduleNote")] Trainer trainer)
        {
            if (id != trainer.TrainerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null) _context.Trainers.Remove(trainer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.TrainerId == id);
        }

        // -------------------- Chức năng Trainer đăng nhập --------------------

        // Danh sách hội viên phụ trách
        public async Task<IActionResult> MyMembers()
        {
            int trainerId = GetCurrentTrainerId();

            var members = await _context.TrainingSchedules
                .Include(ts => ts.Member)
                .Where(ts => ts.TrainerId == trainerId)
                .Select(ts => ts.Member)
                .Distinct()
                .ToListAsync();

            return View(members);
        }

        // Lịch sử tập luyện của hội viên
        public async Task<IActionResult> MemberHistory(int id)
        {
            var sessions = await _context.TrainingSchedules
                .Where(ts => ts.MemberId == id)
                .OrderByDescending(ts => ts.TrainingDate)
                .ToListAsync();

            return View(sessions);
        }

       

        // Xem thông báo gửi đến trainer
        public async Task<IActionResult> MyNotifications()
        {
            int userId = GetCurrentUserId();

            var notifications = await _context.UserNotifications
                .Include(un => un.Notification)
                .Where(un => un.UserId == userId)
                .OrderByDescending(un => un.TimeSend)
                .ToListAsync();

            return View(notifications);
        }

        // -------------------- Hàm hỗ trợ giả định --------------------

        private int GetCurrentTrainerId()
        {
            // Lấy username hiện tại
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                throw new Exception("User chưa đăng nhập");

            // Tìm user trong DB
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username && u.RoleId == 4);

            if (user == null)
                throw new Exception("Không tìm thấy user hoặc user không phải trainer");

            return (int)user.ReferenceId; // đây chính là TrainerId
        }


        private int GetCurrentUserId()
        {
            // TODO: bạn cần lấy từ session, ở đây tạm giả định UserId = 5
            return 5;
        }
    }
}
