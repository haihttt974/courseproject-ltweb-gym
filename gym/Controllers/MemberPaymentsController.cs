using gym.Data;
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
    public class MemberPaymentsController : Controller
    {
        private readonly GymContext _context;

        public MemberPaymentsController(GymContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null || user.ReferenceId == null)
                return Unauthorized();

            var memberId = user.ReferenceId.Value;

            var payments = await _context.MemberPayments
                .Where(mp => mp.MemberId == memberId)
                .Include(mp => mp.Payment)
                .Include(mp => mp.Staff)
                .OrderByDescending(mp => mp.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // GET: MemberPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberPayment = await _context.MemberPayments
                .Include(m => m.Member)
                .Include(m => m.Payment)
                .Include(m => m.Staff)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (memberPayment == null)
            {
                return NotFound();
            }

            return View(memberPayment);
        }

        // GET: MemberPayments/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId");
            ViewData["StaffId"] = new SelectList(_context.Staff, "StaffId", "StaffId");
            return View();
        }

        // POST: MemberPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,PaymentId,StaffId,PaymentDate")] MemberPayment memberPayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", memberPayment.MemberId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", memberPayment.PaymentId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "StaffId", "StaffId", memberPayment.StaffId);
            return View(memberPayment);
        }

        // GET: MemberPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberPayment = await _context.MemberPayments.FindAsync(id);
            if (memberPayment == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", memberPayment.MemberId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", memberPayment.PaymentId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "StaffId", "StaffId", memberPayment.StaffId);
            return View(memberPayment);
        }

        // POST: MemberPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,PaymentId,StaffId,PaymentDate")] MemberPayment memberPayment)
        {
            if (id != memberPayment.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberPaymentExists(memberPayment.MemberId))
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", memberPayment.MemberId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", memberPayment.PaymentId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "StaffId", "StaffId", memberPayment.StaffId);
            return View(memberPayment);
        }

        // GET: MemberPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberPayment = await _context.MemberPayments
                .Include(m => m.Member)
                .Include(m => m.Payment)
                .Include(m => m.Staff)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (memberPayment == null)
            {
                return NotFound();
            }

            return View(memberPayment);
        }

        // POST: MemberPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberPayment = await _context.MemberPayments.FindAsync(id);
            if (memberPayment != null)
            {
                _context.MemberPayments.Remove(memberPayment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberPaymentExists(int id)
        {
            return _context.MemberPayments.Any(e => e.MemberId == id);
        }
    }
}
