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

    public AccountController(GymContext context)
    {
        _context = context;
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

        // Kiểm tra trùng lặp, xử lý NULL
        if (await _context.Users.AnyAsync(u =>
            (u.UserName != null && u.UserName == model.UserName) ||
            (u.Email != null && u.Email == model.Email)))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
            return View(model);
        }

        var user = new User
        {
            RoleId = 2, // Member
            UserName = model.UserName,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Email = model.Email,
            Status = "Hoạt động",
            IsAtive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
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
}