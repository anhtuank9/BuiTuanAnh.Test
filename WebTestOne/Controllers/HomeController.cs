using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebTestOne.Models;

namespace WebTestOne.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /*public IActionResult NameList()
        {
            // Tạo danh sách chuỗi
            List<string> danhSachTen = new List<string>
            {
            "Nguyễn Văn A",
            "Trần Thị B",
            "Lê Văn C",
            "Phạm Thị D"
            };

            // Truyền danh sách làm Model cho View
            return View(danhSachTen);
        }*/

        public IActionResult NameList()
        {
            // Tạo danh sách đối tượng chứa cả Name và Age
            List<PersonModel> danhSachNguoiDung = new List<PersonModel>
    {
        new PersonModel { Name = "Nguyễn Văn A", Age = 25 },
        new PersonModel { Name = "Trần Thị B", Age = 30 },
        new PersonModel { Name = "Lê Văn C", Age = 22 },
        new PersonModel { Name = "Phạm Thị D", Age = 28 }
    };
            // Truyền danh sách Model sang View
            return View(danhSachNguoiDung);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
