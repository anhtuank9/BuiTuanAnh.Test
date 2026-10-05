using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTestOne.Models;

namespace WebTestOne.Controllers
{
    public class AccountController : Controller
    {
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
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tài khoản cứng theo yêu cầu của bài thực hành (admin hoặc user, pass 123456)
                if ((model.Username == "admin" || model.Username == "user") && model.Password == "123456")
                {
                    // 2.1 Tạo danh sách các Claims (Đặc điểm nhận dạng của user)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Username), // userId
                        new Claim(ClaimTypes.Name, model.Username), // Tên hiển thị
                        new Claim(ClaimTypes.Email, $"{model.Username}@viu.edu.vn"), // Email giả định
                        
                        // Nếu tên đăng nhập là admin thì cấp quyền Admin, ngược lại cấp quyền User
                        new Claim(ClaimTypes.Role, model.Username == "admin" ? "Admin" : "User")
                    };

                    // 2.2 Tạo Identity và Principal
                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    // 2.3 Thực hiện cấp phát Cookie đăng nhập
                    await HttpContext.SignInAsync("MyCookieAuth", principal);

                    // 2.4 Đăng nhập thành công thì chuyển hướng về trang cũ hoặc trang chủ
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

        // 3. Xử lý Đăng xuất
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