using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTestOne.Models;
using WebTestOne.Services;

namespace WebTestOne.Controllers;

public class CartController(AppDbContext context, CartService cartService) : Controller
{
    public IActionResult Index()
    {
        return View(cartService.GetCart());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, string? returnUrl = null)
    {
        var book = await context.Saches.AsNoTracking().FirstOrDefaultAsync(s => s.Ms == id);
        if (book is null)
        {
            return NotFound();
        }

        cartService.Add(book);
        TempData["CartMessage"] = $"Đã thêm “{book.TenSach}” vào giỏ hàng.";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeQuantity(int id, int delta)
    {
        cartService.ChangeQuantity(id, delta);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        cartService.Remove(id);
        TempData["CartMessage"] = "Đã xóa sách khỏi giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var cart = cartService.GetCart();
        if (cart.Items.Count == 0)
        {
            TempData["CartError"] = "Giỏ hàng đang trống.";
            return RedirectToAction(nameof(Index));
        }

        if (User.Identity?.IsAuthenticated != true)
        {
            TempData["AuthRequired"] = "Vui lòng đăng nhập hoặc đăng ký tài khoản trước khi đặt hàng.";
            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl = Url.Action(nameof(Index), "Cart") });
        }

        int? customerId = null;
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedCustomerId))
        {
            customerId = parsedCustomerId;
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var order = new DonDatHang
            {
                Mkh = customerId,
                NgayDatHang = DateTime.Now,
                TriGia = cart.GrandTotal,
                DaGiaoHang = false
            };

            foreach (var item in cart.Items)
            {
                order.CtDatHangs.Add(new CtDatHang
                {
                    Ms = item.BookId,
                    SoLuong = item.Quantity,
                    DonGia = (double)item.UnitPrice,
                    ThanhTien = (double)item.LineTotal
                });
            }

            context.DonDatHangs.Add(order);

            var bookIds = cart.Items.Select(item => item.BookId).ToList();
            var books = await context.Saches.Where(s => bookIds.Contains(s.Ms)).ToListAsync();
            foreach (var book in books)
            {
                var quantity = cart.Items.First(item => item.BookId == book.Ms).Quantity;
                book.SoLuongBan = (book.SoLuongBan ?? 0) + quantity;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            cartService.Clear();
            TempData["OrderSuccess"] = $"Đặt hàng thành công. Mã đơn hàng của bạn là #{order.Sdh}.";
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["CartError"] = "Chưa thể lưu đơn hàng. Vui lòng thử lại sau.";
        }

        return RedirectToAction(nameof(Index));
    }
}
