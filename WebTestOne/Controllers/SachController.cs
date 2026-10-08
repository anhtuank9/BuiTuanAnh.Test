using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTestOne.Models;
using System.Data;
using WebTestOne.Services;

namespace WebTestOne.Controllers
{
    public class SachController : Controller
    {
        private readonly AppDbContext _context;

        public SachController(AppDbContext context)
        {
            _context = context;
        }

        public ActionResult ChuDe()
        {
            List<ChuDe> dsChuDe = _context.ChuDes.ToList();
            return View(dsChuDe);
        }

        public ActionResult ChuDeEdit(int id)
        {
            ChuDe? chuDe = new ChuDe { Mcd = 0, TenChuDe = "" };

            if (id == 0)
            {
                return View(chuDe);
            }

            chuDe = _context.ChuDes.Find(id);
            if (chuDe == null)
            {
                return RedirectToAction(nameof(ChuDe));
            }

            return View(chuDe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuDeEdit(ChuDe chude)
        {
            if (ModelState.IsValid)
            {
                if (chude.Mcd == 0)
                {
                    _context.ChuDes.Add(chude);
                    TempData["SuccessMessage"] = "Thêm mới thành công!";
                }
                else
                {
                    _context.ChuDes.Update(chude);
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                }

                _ = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ChuDe));
            }

            return View(chude);
        }

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

        public async Task<IActionResult> Index(int? chuDeId)
        {
            var query = _context.Saches
                .AsNoTracking()
                .Include(s => s.MnxbNavigation)
                .Include(s => s.McdNavigation)
                .Include(s => s.ThamGia)
                    .ThenInclude(tg => tg.MtgNavigation)
                .AsQueryable();

            if (chuDeId.HasValue)
            {
                query = query.Where(s => s.Mcd == chuDeId.Value);
            }

            var books = await query.OrderByDescending(s => s.NgayCapNhat).ToListAsync();
            var resultList = books.Select(s => new SachViewModel
            {
                Ms = s.Ms,
                TenSach = s.TenSach,
                DonGia = s.DonGia,
                HinhMinhHoa = s.HinhMinhHoa,
                MoTa = s.MoTa,
                TenNXB = s.MnxbNavigation?.TenNhaXuatBan,
                TenChuDe = s.McdNavigation?.TenChuDe,
                TacGia = string.Join(", ", s.ThamGia.Select(tg => tg.MtgNavigation.TenTacGia)),
                ImageUrl = StoreImage.Book(s.HinhMinhHoa)
            }).ToList();

            ViewBag.SelectedTopicId = chuDeId;
            ViewBag.PageHeading = chuDeId.HasValue
                ? await _context.ChuDes.Where(cd => cd.Mcd == chuDeId.Value).Select(cd => cd.TenChuDe).FirstOrDefaultAsync()
                    ?? "Danh mục sách"
                : "Tất cả sách";

            return View("SachCardView", resultList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Saches
                .Include(s => s.MnxbNavigation)
                .Include(s => s.McdNavigation)
                .Include(s => s.ThamGia)
                    .ThenInclude(tg => tg.MtgNavigation)
                .FirstOrDefaultAsync(s => s.Ms == id);

            if (book is null)
            {
                return NotFound();
            }

            book.SoLanXem = (book.SoLanXem ?? 0) + 1;
            await _context.SaveChangesAsync();

            return View(new BookDetailViewModel
            {
                Sach = book,
                NhaXuatBan = book.MnxbNavigation?.TenNhaXuatBan ?? "Đang cập nhật",
                ChuDe = book.McdNavigation?.TenChuDe ?? "Đang cập nhật",
                TacGia = string.Join(", ", book.ThamGia.Select(tg => tg.MtgNavigation.TenTacGia)),
                ImageUrl = StoreImage.Book(book.HinhMinhHoa)
            });
        }

        private List<Dictionary<string, object>> ExecuteRawSql(string sqlQuery)
        {
            var resultList = new List<Dictionary<string, object>>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;

                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i);
                        }
                        resultList.Add(row);
                    }
                }
                _context.Database.CloseConnection();
            }
            return resultList;
        }

        public IActionResult QueryDemo(int? id)
        {
            List<SachQuery> queries = new List<SachQuery>
            {
                new SachQuery { Id = 1, QueryName = "1. Lấy danh mục chủ đề" },
                new SachQuery { Id = 2, QueryName = "2. Chủ đề và số lượng sách" },
                new SachQuery { Id = 3, QueryName = "3. Chủ đề có sách" },
                new SachQuery { Id = 4, QueryName = "4. Sách thuộc chủ đề 5" },
                new SachQuery { Id = 5, QueryName = "5. TOP 5 sách mới cập nhật" },
                new SachQuery { Id = 6, QueryName = "6. TOP 5 sách bán chạy nhất" },
                new SachQuery { Id = 7, QueryName = "7. Quảng cáo còn hạn" },
                new SachQuery { Id = 8, QueryName = "8. Tác giả sách mã số 2" },
                new SachQuery { Id = 9, QueryName = "9. Đơn hàng đã giao" }
            };

            ViewBag.Queries = new SelectList(queries, "Id", "QueryName", id);

            if (id.HasValue)
            {
                string sql = "";

                switch (id.Value)
                {
                    case 1:
                        sql = @"SELECT cd.Mcd, cd.Ten_chu_de 
                                FROM dbo.CHU_DE AS cd ORDER BY cd.Ten_chu_de;";
                        break;
                    case 2:
                        sql = @"SELECT cd.Mcd, cd.Ten_chu_de, COUNT(s.Ms) AS So_luong_sach 
                                FROM dbo.CHU_DE AS cd LEFT JOIN dbo.SACH AS s ON cd.Mcd = s.Mcd 
                                GROUP BY cd.Mcd, cd.Ten_chu_de 
                                HAVING COUNT(s.Ms) > 0 
                                ORDER BY cd.Ten_chu_de;";
                        break;
                    case 3:
                        sql = @"SELECT DISTINCT cd.Mcd, cd.Ten_chu_de 
                                FROM dbo.CHU_DE AS cd INNER JOIN dbo.SACH AS s ON cd.Mcd = s.Mcd 
                                ORDER BY cd.Ten_chu_de;";
                        break;
                    case 4:
                        sql = @"SELECT s.Ms, s.Ten_sach, s.Don_gia, s.Don_vi_tinh, s.Mo_ta, s.Hinh_minh_hoa, 
                                       s.Mcd, s.Mnxb, s.Ngay_cap_nhat, s.So_luong_ban, s.So_lan_xem 
                                FROM dbo.SACH AS s WHERE s.Mcd = 5 ORDER BY s.Ten_sach;";
                        break;
                    case 5:
                        sql = @"SELECT TOP 5 s.Ms, s.Ten_sach, s.Hinh_minh_hoa, s.Ngay_cap_nhat 
                                FROM dbo.SACH AS s ORDER BY s.Ngay_cap_nhat DESC;";
                        break;
                    case 6:
                        sql = @"SELECT TOP 5 s.Ms, s.Ten_sach, s.Hinh_minh_hoa, SUM(ct.So_luong) AS Tong_so_luong_ban 
                                FROM dbo.SACH AS s INNER JOIN dbo.CT_DAT_HANG AS ct ON s.Ms = ct.Ms 
                                GROUP BY s.Ms, s.Ten_sach, s.Hinh_minh_hoa ORDER BY Tong_so_luong_ban DESC;";
                        break;
                    case 7:
                        sql = @"SELECT qc.STT, qc.TenCty, qc.Hinh_Minh_Hoa, qc.HREF, qc.Ngay_bat_dau, qc.Ngay_het_han 
                                FROM dbo.QUANG_CAO AS qc WHERE GETDATE() BETWEEN qc.Ngay_bat_dau AND qc.Ngay_het_han 
                                ORDER BY qc.STT;";
                        break;
                    case 8:
                        sql = @"SELECT tg.Mtg, tg.Ten_tac_gia, tg.Dia_chi, tg.Dien_thoai, tg_gia.Vai_tro 
                                FROM dbo.TAC_GIA AS tg INNER JOIN dbo.THAM_GIA AS tg_gia ON tg.Mtg = tg_gia.Mtg 
                                WHERE tg_gia.Ms = 2 ORDER BY tg.Ten_tac_gia;";
                        break;
                    case 9:
                        sql = @"SELECT dh.Sdh, dh.Mkh, kh.Ho_ten, dh.Ngay_dat_hang, dh.Tri_gia, dh.Da_giao_hang, dh.Ngay_giao_hang 
                                FROM dbo.DON_DAT_HANG AS dh INNER JOIN dbo.KHACH_HANG AS kh ON dh.Mkh = kh.Mkh 
                                WHERE dh.Da_giao_hang = 1 ORDER BY dh.Ngay_giao_hang DESC;";
                        break;
                }

                if (!string.IsNullOrEmpty(sql))
                {
                    var result = ExecuteRawSql(sql);
                    return View(result);
                }
            }

            return View(null);
        }
    }
}
