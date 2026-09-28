using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTestOne.Models;

namespace WebTestOne.Controllers
{
    public class SachController : Controller
    {
        // 1. Khai báo DbContext dùng chung cho toàn bộ Controller
        private readonly AppDbContext _context = new AppDbContext();

        #region "ChuDe"

        // GET: Sach/ChuDe (Hiển thị danh sách chủ đề)
        public ActionResult ChuDe()
        {
            List<ChuDe> dsChuDe = _context.ChuDes.ToList();
            return View(dsChuDe);
        }

        // GET: Sach/ChuDeEdit/5 hoặc Sach/ChuDeEdit/0 (Mở form Thêm hoặc Sửa)
        public ActionResult ChuDeEdit(int id)
        {
            // Khởi tạo đối tượng mặc định cho trường hợp Thêm mới
            ChuDe? chuDe = new ChuDe { Mcd = 0, TenChuDe = "" };

            // Nếu id == 0 => Mở form Thêm mới chủ đề
            if (id == 0)
            {
                return View(chuDe);
            }

            // Nếu id > 0 => Tìm chủ đề hiện tại trong CSDL để đổ lên form Sửa
            chuDe = _context.ChuDes.Find(id);
            if (chuDe == null)
            {
                return RedirectToAction(nameof(ChuDe));
            }

            return View(chuDe);
        }

        // POST: Sach/ChuDeEdit (Nhận dữ liệu từ form và Lưu/Cập nhật vào CSDL)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuDeEdit(ChuDe chude)
        {
            if (ModelState.IsValid)
            {
                if (chude.Mcd == 0) // Thêm mới
                {
                    _context.ChuDes.Add(chude);
                    TempData["SuccessMessage"] = "Thêm mới thành công!";
                }
                else // Cập nhật
                {
                    _context.ChuDes.Update(chude);
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                }

                // Lưu thay đổi vào SQL Server
                _ = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ChuDe));
            }

            // Dữ liệu không hợp lệ => trả lại form kèm thông báo lỗi
            return View(chude);
        }

        // POST: Sach/ChuDeDelete/5 (Xóa chủ đề khỏi CSDL)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuDeDelete(int id)
        {
            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe != null)
            {
                _context.ChuDes.Remove(chuDe);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa chủ đề thành công!";
            }

            return RedirectToAction(nameof(ChuDe));
        }

        #endregion

        #region "Sach"

        // GET: Sach hoặc Sach/Index
        public ActionResult Index()
        {
            AppDbContext context = new AppDbContext();
            List<Sach> dsSach = context.Saches.ToList();
            return View(dsSach);
        }

        #endregion
    }
}