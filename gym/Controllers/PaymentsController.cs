using DinkToPdf;
using DinkToPdf.Contracts;
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
    public class PaymentsController : Controller
    {
        private readonly GymContext _context;
        private readonly EmailService _emailService;
        private readonly IConverter _pdfConverter;

        public PaymentsController(GymContext context, EmailService emailService, IConverter pdfConverter)
        {
            _context = context;
            _emailService = emailService;
            _pdfConverter = pdfConverter;
        }


        // GET: Payments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Payments.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,Amount,Method")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,Amount,Method")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<IActionResult> Pay(int memberId, int packageId)
        {
            var member = await _context.Members.FindAsync(memberId);
            var package = await _context.Packages.FindAsync(packageId);

            if (member == null || package == null)
                return NotFound();

            var viewModel = new PaymentViewModel
            {
                MemberId = memberId,
                PackageId = packageId,
                MemberName = member.FullName,
                PackageName = package.Name,
                Amount = (decimal)package.Price
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var staffUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (staffUser == null || staffUser.ReferenceId == null)
                return Unauthorized();

            var staffId = staffUser.ReferenceId.Value;

            // Tạo bản ghi mới trong bảng Payment
            var payment = new Payment
            {
                Amount = model.Amount,
                Method = model.Method
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // Lưu để có PaymentId

            // Tạo bản ghi trong MemberPayment
            var memberPayment = new MemberPayment
            {
                MemberId = model.MemberId,
                PaymentId = payment.PaymentId,
                StaffId = staffId,
                PaymentDate = DateTime.Now
            };
            _context.MemberPayments.Add(memberPayment);

            // Cập nhật MemberPackage
            var memberPackage = await _context.MemberPakages.FirstOrDefaultAsync(
                x => x.MemberId == model.MemberId && x.PackageId == model.PackageId && x.IsPaid == false);

            if (memberPackage != null)
            {
                memberPackage.IsPaid = true;
                memberPackage.IsActive = true;
            }

            await _context.SaveChangesAsync();

            var member = await _context.Members.FindAsync(model.MemberId);
            if (member != null && !string.IsNullOrEmpty(member.Email))
            {
                string subject = "Xác nhận thanh toán gói tập gym";
                string body = $@"
                    <h3>Chào {member.FullName},</h3>
                    <p>Bạn đã thanh toán thành công cho gói tập <strong>{model.PackageName}</strong> với số tiền <strong>{model.Amount.ToString("N0")} VNĐ</strong>.</p>
                    <p>Gói tập của bạn đã được kích hoạt.</p>
                    <p>Trân trọng,<br>GYM Club</p>";

                await _emailService.SendEmailAsync(member.Email, subject, body);
            }


            TempData["Success"] = "Thanh toán thành công và gói đã được kích hoạt!";
            return RedirectToAction("Index", "Packages");
        }
        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirst("MemberId")!.Value);

            var history = await _context.MemberPayments
                .Include(mp => mp.Payment)
                .Include(mp => mp.Staff)
                .Where(mp => mp.MemberId == userId)
                .OrderByDescending(mp => mp.PaymentDate)
                .Select(mp => new
                {
                    mp.PaymentId,
                    mp.PaymentDate,
                    Amount = mp.Payment.Amount,
                    StaffName = mp.Staff.FullName
                })
                .ToListAsync();

            return View(history);
        }

        public async Task<IActionResult> DownloadInvoice(int id)
        {
            var payment = await _context.MemberPayments
                .Include(mp => mp.Member)
                .Include(mp => mp.Payment)
                .Include(mp => mp.Staff)
                .FirstOrDefaultAsync(mp => mp.PaymentId == id);

            if (payment == null)
                return NotFound();

            string html = $@"
                <h1>GYM CLUB - HÓA ĐƠN THANH TOÁN</h1>
                <p><strong>Hội viên:</strong> {payment.Member.FullName}</p>
                <p><strong>Nhân viên thu:</strong> {payment.Staff.FullName}</p>
                <p><strong>Ngày thanh toán:</strong> {payment.PaymentDate?.ToString("dd/MM/yyyy")}</p>
                <p><strong>Số tiền:</strong> {payment.Payment.Amount?.ToString("N0")} VNĐ</p>
                <br/><p>Xin cảm ơn quý khách!</p>
            ";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
                Objects = { new ObjectSettings { HtmlContent = html } }
            };

            var pdf = _pdfConverter.Convert(doc);
            return File(pdf, "application/pdf", $"Invoice_{id}.pdf");
        }
    }
}
