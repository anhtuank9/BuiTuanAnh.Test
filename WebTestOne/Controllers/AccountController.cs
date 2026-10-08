using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTestOne.Models;
using Microsoft.EntityFrameworkCore;

namespace WebTestOne.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị form đăng nhập (GET)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Lưu lại URL trước đó để đăng nhập xong quay lại đúng trang đang xem dở
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // 2. Xử lý dữ liệu khi bấm nút Đăng nhập (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.KhachHangs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(kh => kh.TenDangNhap == model.Username && kh.MatKhau == model.Password);
                var isDemoAccount = (model.Username == "admin" || model.Username == "user")
                    && model.Password == "123456";

                if (customer is not null || isDemoAccount)
                {
                    var displayName = customer?.HoTen ?? model.Username;
                    var identifier = customer?.Mkh.ToString() ?? model.Username;
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, identifier),
                        new Claim(ClaimTypes.Name, displayName),
                        new Claim(ClaimTypes.Email, customer?.Email ?? $"{model.Username}@viu.edu.vn"),
                        new Claim(ClaimTypes.Role, model.Username == "admin" ? "Admin" : "User")
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("MyCookieAuth", principal);
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                // Báo lỗi nếu sai tài khoản/mật khẩu
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            if (await _context.KhachHangs.AnyAsync(kh => kh.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập này đã được sử dụng.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var customer = new KhachHang
            {
                HoTen = model.HoTen.Trim(),
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = model.MatKhau,
                Email = model.Email?.Trim(),
                DienThoai = model.DienThoai?.Trim(),
                DiaChi = model.DiaChi?.Trim(),
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh
            };

            _context.KhachHangs.Add(customer);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đăng ký thành công. Bạn có thể đăng nhập ngay bây giờ.";
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        // 3. Xử lý Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Xóa Cookie
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }

        // 4. Trang thông báo lỗi khi không có quyền truy cập (Access Denied)
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
