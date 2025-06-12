using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gym.Data;
using gym.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace gym.Controllers;

public class AccountController : Controller
{
    private readonly GymContext _context;
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;
    public AccountController(GymContext context, EmailService emailService, IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
    }

    // GET: Đăng nhập
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Đăng nhập
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == model.UserName && u.IsAtive == true);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim("UserId", user.UserId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    // GET: Đăng ký
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Đăng ký
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");
            return View(model);
        }

        if (await _context.Users.AnyAsync(u =>
            (u.UserName != null && u.UserName == model.UserName) ||
            (u.Email != null && u.Email == model.Email)))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
            return View(model);
        }

        // Lưu thông tin tạm thời vào session (chưa lưu DB)
        HttpContext.Session.SetString("TempRegister_FullName", model.FullName);
        HttpContext.Session.SetString("TempRegister_DateOfBirth", model.DateOfBirth.ToString("yyyy-MM-dd"));
        HttpContext.Session.SetString("TempRegister_Sex", model.Sex.ToString());
        HttpContext.Session.SetString("TempRegister_Phone", model.Phone);
        HttpContext.Session.SetString("TempRegister_Address", model.Address);
        HttpContext.Session.SetString("TempRegister_UserName", model.UserName);
        HttpContext.Session.SetString("TempRegister_Password", model.Password);
        HttpContext.Session.SetString("TempRegister_Email", model.Email);

        // Tạo mã OTP
        var otpCode = new Random().Next(100000, 999999).ToString();
        HttpContext.Session.SetString("RegisterOtp", otpCode);
        HttpContext.Session.SetString("RegisterEmail", model.Email);

        // Gửi mail OTP
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OtpRegisterTemplate.html");
        string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);
        emailBody = emailBody.Replace("{{FULLNAME}}", model.FullName)
                             .Replace("{{OTP}}", otpCode);

        var emailService = new EmailService(_configuration);
        await emailService.SendEmailAsync(model.Email, "Xác nhận tài khoản GYM Club", emailBody);

        return RedirectToAction("VerifyOtp", new { email = model.Email });
    }

    // POST: Đăng xuất
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    // GET: Quên mật khẩu
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST: Quên mật khẩu
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.IsAtive == true);
        if (user == null)
        {
            ModelState.AddModelError("", "Email không tồn tại");
            return View(model);
        }

        // Giả lập gửi email đặt lại mật khẩu
        // TODO: Tích hợp dịch vụ email (e.g., SendGrid) để gửi link đặt lại mật khẩu
        ViewBag.Message = "Link đặt lại mật khẩu đã được gửi đến email của bạn.";
        return View();
    }

    // GET: Chỉnh sửa trang cá nhân
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditProfile()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new EditProfileDto
        {
            UserName = user.UserName,
            Email = user.Email
        };
        return View(model);
    }

    // POST: Chỉnh sửa trang cá nhân
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditProfile(EditProfileDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = int.Parse(User.FindFirst("UserId")?.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (await _context.Users.AnyAsync(u => (u.UserName == model.UserName || u.Email == model.Email) && u.UserId != userId))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
            return View(model);
        }

        user.UserName = model.UserName;
        user.Email = model.Email;
        if (!string.IsNullOrEmpty(model.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult VerifyOtp(string email)
    {
        return View(new OtpVerificationDto { Email = email });
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(OtpVerificationDto model)
    {
        var sessionOtp = HttpContext.Session.GetString("RegisterOtp");
        var sessionEmail = HttpContext.Session.GetString("RegisterEmail");

        if (model.Email != sessionEmail || model.OtpCode != sessionOtp)
        {
            ModelState.AddModelError("", "Mã OTP không đúng hoặc đã hết hạn.");
            return View(model);
        }

        try
        {
            var member = new Member
            {
                FullName = HttpContext.Session.GetString("TempRegister_FullName"),
                DateOfBirth = DateTime.Parse(HttpContext.Session.GetString("TempRegister_DateOfBirth")),
                Sex = bool.Parse(HttpContext.Session.GetString("TempRegister_Sex")),
                Phone = HttpContext.Session.GetString("TempRegister_Phone"),
                Address = HttpContext.Session.GetString("TempRegister_Address"),
                CreateDate = DateTime.Now
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            var user = new User
            {
                RoleId = 2,
                UserName = HttpContext.Session.GetString("TempRegister_UserName"),
                Password = BCrypt.Net.BCrypt.HashPassword(HttpContext.Session.GetString("TempRegister_Password")),
                Email = sessionEmail,
                Status = "Hoạt động",
                IsAtive = true,
                ReferenceId = member.MemberId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Clear session
            HttpContext.Session.Remove("RegisterOtp");
            HttpContext.Session.Remove("RegisterEmail");
            HttpContext.Session.Remove("TempRegister_FullName");
            HttpContext.Session.Remove("TempRegister_DateOfBirth");
            HttpContext.Session.Remove("TempRegister_Sex");
            HttpContext.Session.Remove("TempRegister_Phone");
            HttpContext.Session.Remove("TempRegister_Address");
            HttpContext.Session.Remove("TempRegister_UserName");
            HttpContext.Session.Remove("TempRegister_Password");
            HttpContext.Session.Remove("TempRegister_Email");

            TempData["Success"] = "✅ Xác thực OTP thành công. Bạn có thể đăng nhập!";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "❌ Có lỗi xảy ra khi lưu thông tin: " + ex.Message);
            return View(model);
        }
    }
}