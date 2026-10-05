using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebTestOne.Controllers
{
    public class AdminController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            // 1. Kiểm tra trạng thái đăng nhập
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            // 2. Trích xuất Claims
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? email = User.FindFirst(ClaimTypes.Email)?.Value;
            string? role = User.FindFirst(ClaimTypes.Role)?.Value;

            // 3. Kiểm tra Phân quyền
            bool isAdmin = User.IsInRole("Admin");

            return Ok(new
            {
                UserId = userId,
                Email = email,
                Role = role,
                IsAdmin = isAdmin
            });
        }
    }
}