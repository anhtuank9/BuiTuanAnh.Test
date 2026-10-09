# Dự án ASP.NET Core MVC - WebTestOne

WebTestOne là website **Book Online** được xây dựng bằng ASP.NET Core MVC, Entity Framework Core và SQL Server. Giao diện được hoàn thiện theo bố cục bài tập: banner đầu trang, menu ngang, danh mục chủ đề bên trái, nội dung chính ở giữa, giỏ hàng/banner đối tác/sách bán chạy bên phải và footer ở cuối trang.

## 1. Nội dung yêu cầu đề bài

Thiết kế và lập trình một website bán sách đơn giản với các yêu cầu sau.

### Bố cục trang dùng chung

- Vùng banner hiển thị banner của website.
- Vùng menu ngang gồm Trang chủ, Đăng ký và Đăng nhập.
- Vùng Chủ đề sách hiển thị các chủ đề; khi chọn một chủ đề, vùng nội dung chính hiển thị sách thuộc chủ đề đó.
- Vùng Sách mới hiển thị các sách mới cập nhật.
- Vùng Thông tin giỏ hàng hiển thị số cuốn sách và số tiền cần thanh toán; khi nhấn vào sẽ mở trang chi tiết giỏ hàng.
- Vùng Quảng cáo 1 hiển thị 3 ảnh quảng cáo kèm liên kết.
- Vùng Quảng cáo 2 hiển thị các banner vẫn còn thời hạn theo dữ liệu trong bảng `QUANG_CAO`.
- Vùng Sách bán chạy hiển thị 5 đầu sách có số lượng bán nhiều nhất; tên sách liên kết tới trang chi tiết.
- Vùng nội dung chính thay đổi theo từng trang, có thể là danh mục sách, chi tiết sách hoặc nội dung chức năng khác.
- Footer hiển thị bản quyền, địa chỉ và số điện thoại liên hệ.

### Các trang chức năng

- Trang danh mục sách hiển thị sách theo chủ đề, gồm tên sách, tác giả, nhà xuất bản, giá và nút mua hàng.
- Khi nhấn mua, sách được thêm vào giỏ hàng nếu chưa có.
- Trang chi tiết sách hiển thị thông tin đầy đủ khi người dùng nhấn tên hoặc hình ảnh sách.
- Trang giỏ hàng cho phép xem chi tiết, tăng giảm số lượng, xóa một mục hàng và xem tổng tiền.
- Nút Đặt hàng cập nhật đơn hàng và chi tiết đơn hàng vào cơ sở dữ liệu.

### Tiêu chí đánh giá trong đề bài

| Nội dung | Điểm |
|---|---:|
| Giao diện đẹp, hợp lý | 1,0 |
| Tạo trang dùng chung theo bố cục yêu cầu | 1,0 |
| Hiển thị danh mục sách theo chủ đề | 2,0 |
| Hiển thị sách bán chạy và sách mới | 1,0 |
| Hiển thị quảng cáo 1 và quảng cáo 2 | 1,0 |
| Thêm hàng và hiển thị thông tin giỏ hàng | 2,0 |
| Thay đổi số lượng, xóa mục hàng và tính tổng tiền | 1,0 |
| Cập nhật đơn hàng | 1,0 |

> Dự án sử dụng ASP.NET Core MVC nên `_Layout.cshtml`, Razor Views, View Components và bảng HTML được dùng thay cho MasterPage, DataList và GridView của ASP.NET Web Forms.

## 2. Cấu trúc thư mục

```text
BuiTuanAnh.Test/
│
├── WebTestOne/
│   ├── Controllers/
│   │   ├── AccountController.cs     # Đăng nhập, đăng ký và đăng xuất
│   │   ├── AdminController.cs       # Chức năng dành cho quản trị viên
│   │   ├── CartController.cs        # Giỏ hàng và đặt hàng
│   │   ├── HomeController.cs        # Trang chủ, Privacy và NameList
│   │   └── SachController.cs        # Danh mục, chi tiết sách, chủ đề và truy vấn mẫu
│   │
│   ├── Models/                       # Entity, ViewModel và AppDbContext
│   ├── Services/
│   │   └── StoreHelpers.cs          # Giỏ hàng, chuẩn hóa và dự phòng đường dẫn ảnh
│   ├── ViewComponents/
│   │   └── StoreViewComponents.cs   # Chủ đề, sách mới, quảng cáo, giỏ hàng, bán chạy
│   │
│   ├── Views/
│   │   ├── Account/                 # Đăng nhập và đăng ký
│   │   ├── Cart/                    # Trang giỏ hàng
│   │   ├── Home/                    # Trang chủ, Privacy và NameList
│   │   ├── Sach/                    # Danh sách, chi tiết và quản lý sách/chủ đề
│   │   └── Shared/
│   │       ├── Components/          # View của các View Component
│   │       └── _Layout.cshtml       # Header, menu, bố cục ba cột và footer
│   │
│   ├── wwwroot/
│   │   ├── css/site.css             # Giao diện chính, responsive và UTF-8
│   │   ├── js/site.js               # Menu mobile và carousel quảng cáo
│   │   └── images/                  # Ảnh sách, banner GIF/JPG và ảnh dự phòng
│   │
│   ├── Properties/launchSettings.json
│   ├── appsettings.json
│   ├── Program.cs
│   └── WebTestOne.csproj
│
├── .gitattributes
├── .gitignore
├── README.md
└── WebTestOne.slnx
```

## 3. Cách khởi chạy trực tuyến trên GitHub Codespaces

1. Mở repository `https://github.com/anhtuank9/BuiTuanAnh.Test`.
2. Chọn **Code** → **Codespaces** → **Create codespace on main**.
3. Cấu hình connection string `QLBanSach` tới SQL Server mà Codespace có thể truy cập.
4. Mở Terminal và chạy:

   ```bash
   cd WebTestOne
   dotnet restore
   dotnet run --no-launch-profile --urls http://0.0.0.0:5000
   ```

5. Mở tab **Ports**, tìm cổng `5000` và chọn **Open in Browser**.
6. Chỉ chuyển **Port Visibility** sang **Public** khi cần chia sẻ website.

Nhấn `Ctrl + C` trong Terminal để dừng ứng dụng.
