using gym.Data;
using gym.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace gym.Controllers
{
    public class PackagesController : Controller
    {
        private readonly GymContext _context;

        public PackagesController(GymContext context)
        {
            _context = context;
        }

        // GET: Packages
        public async Task<IActionResult> Index()
        {
            var activePackages = await _context.Packages
                .Where(p => p.IsActive)
                .Include(p => p.MemberPakages)
                .ToListAsync();

            return View(activePackages);
        }

        public async Task<IActionResult> Admin()
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
        [HttpGet]
        public async Task<IActionResult> RegisterPackage(int packageId)
        {
            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
                return NotFound();

            return View(package);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRegister(int packageId)
        {
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new Exception("❌ Không xác định được tài khoản.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.RoleId == 2);
            if (user == null)
                throw new Exception($"❌ Không tìm thấy user có username = {username}");

            int memberId = (int)user.ReferenceId;
            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
                throw new Exception($"❌ Không tìm thấy Member với ID = {memberId}");

            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
                throw new Exception($"❌ Gói tập không tồn tại.");

            var hasActive = await _context.MemberPakages
                .AnyAsync(mp => mp.MemberId == memberId && mp.PackageId == packageId && mp.IsActive==true);

            if (hasActive)
            {
                TempData["Error"] = "Bạn đã đăng ký gói này rồi.";
                return RedirectToAction(nameof(Index));
            }

            var memberPackage = new MemberPakage
            {
                MemberId = memberId,
                PackageId = packageId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(package.DurationInDays ?? 0),
                IsPaid = false,
                IsActive = true
            };

            _context.MemberPakages.Add(memberPackage);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}