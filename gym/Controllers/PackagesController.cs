using gym.Data;
using gym.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gym.Controllers
{
    public class PackagesController : Controller
    {
        private readonly GymContext _context;
        private readonly EmailService _emailService;

        public PackagesController(GymContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Packages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Packages.ToListAsync());
        }

        // GET: Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.PackageId == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // GET: Packages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PackageId,Name,Type,Price,DurationInDays,Description")] Package package)
        {
            if (ModelState.IsValid)
            {
                _context.Add(package);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(package);
        }

        // GET: Packages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PackageId,Name,Type,Price,DurationInDays,Description")] Package package)
        {
            if (id != package.PackageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.PackageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(package);
        }

        // GET: Packages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.PackageId == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                _context.Packages.Remove(package);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.PackageId == id);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Register(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null) return NotFound();

            var model = new RegisterPackageViewModel
            {
                PackageId = package.PackageId,
                PackageName = package.Name,
                Price = (decimal)package.Price,
                DurationInDays = (int)package.DurationInDays,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays((int)package.DurationInDays)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Register(RegisterPackageViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Lấy user hiện tại
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null || user.ReferenceId == null) return Unauthorized();

            var memberId = user.ReferenceId.Value;

            // Kiểm tra xem đã có gói còn hoạt động không
            var existing = await _context.MemberPakages
                .FirstOrDefaultAsync(mp => mp.MemberId == memberId && mp.IsActive == true && mp.EndDate >= DateTime.Today);

            if (existing != null)
            {
                TempData["Error"] = "Bạn đang có một gói tập còn hiệu lực.";
                return RedirectToAction("Index");
            }

            // Tạo bản ghi mới trong MemberPackage
            var newMemberPackage = new MemberPackage
            {
                MemberId = memberId,
                PackageId = model.PackageId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsPaid = model.Price == 0, // Nếu miễn phí thì mặc định đã thanh toán
                IsActive = model.Price == 0,
            };

            _context.MemberPakages.Add(newMemberPackage);
            await _context.SaveChangesAsync();

            if (model.Price > 0)
            {
                // Chuyển hướng đến trang thanh toán nếu có phí
                return RedirectToAction("Pay", "Payments", new { memberId = memberId, packageId = model.PackageId });
            }

            TempData["Success"] = "Đăng ký gói tập thành công!";
            return RedirectToAction("MyPackages");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyPackages()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null || user.ReferenceId == null) return Unauthorized();

            var memberId = user.ReferenceId.Value;

            var packages = await _context.MemberPakages
                .Include(mp => mp.Package)
                .Where(mp => mp.MemberId == memberId)
                .OrderByDescending(mp => mp.StartDate)
                .ToListAsync();

            return View(packages);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CurrentPackage()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null || user.ReferenceId == null)
                return Unauthorized();

            var memberId = user.ReferenceId.Value;

            var current = await _context.MemberPakages
                .Include(mp => mp.Package)
                .Where(mp => mp.MemberId == memberId && mp.IsActive == true && mp.EndDate >= DateTime.Today)
                .FirstOrDefaultAsync();

            if (current == null)
            {
                ViewBag.Message = "Bạn chưa có gói tập nào đang hoạt động.";
                return View(null);
            }

            return View(current);
        }

        public async Task<IActionResult> Current()
        {
            var memberId = int.Parse(User.FindFirst("MemberId")!.Value);

            var current = await _context.MemberPakages
                .Include(mp => mp.Package)
                .Where(mp => mp.MemberId == memberId)
                .OrderByDescending(mp => mp.EndDate)
                .FirstOrDefaultAsync();

            return View(current);
        }
        [HttpPost]
        public async Task<IActionResult> Renew(int packageId)
        {
            var memberId = int.Parse(User.FindFirst("MemberId")!.Value);

            var lastPackage = await _context.MemberPakages
                .Where(mp => mp.MemberId == memberId && mp.PackageId == packageId)
                .OrderByDescending(mp => mp.EndDate)
                .FirstOrDefaultAsync();

            if (lastPackage == null)
                return NotFound();

            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
                return NotFound();

            // Ngày bắt đầu mới là ngày sau khi gói cũ kết thúc (hoặc hôm nay nếu đã hết hạn lâu)
            var startDate = (lastPackage.EndDate ?? DateTime.Today).AddDays(1);
            var endDate = startDate.AddDays(package.Duration);

            // Tạo MemberPackage mới
            var newPkg = new MemberPackage
            {
                MemberId = memberId,
                PackageId = packageId,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                IsPaid = true
            };
            _context.MemberPakages.Add(newPkg);

            // Ghi thanh toán
            var payment = new Payment
            {
                Amount = package.Price
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var staff = await _context.Staff.FirstOrDefaultAsync(); // giả định lấy NV đầu tiên

            var memberPayment = new MemberPayment
            {
                MemberId = memberId,
                PaymentId = payment.PaymentId,
                StaffId = staff?.StaffId ?? 1,
                PaymentDate = DateTime.Now
            };
            _context.MemberPayments.Add(memberPayment);
            await _context.SaveChangesAsync();

            // Gửi email
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                await _emailService.SendEmailAsync(member.Email, "Gia hạn gói tập thành công",
                    $"Chào {member.FullName},<br> Bạn đã gia hạn gói <strong>{package.Name}</strong> thành công đến ngày <strong>{endDate:dd/MM/yyyy}</strong>.<br> Cảm ơn bạn!");
            }

            TempData["Success"] = "Gia hạn thành công!";
            return RedirectToAction("Current");
        }

    }
}
