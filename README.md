# Dự án ASP.NET Core MVC - WebTestOne

## Cấu trúc thư mục

```text
BuiTuanAnh.Test/
│
├── WebTestOne/
│   ├── Controllers/
│   │   └── HomeController.cs        # Chứa action NameList() khởi tạo danh sách và truyền Model
│   │
│   ├── Models/
│   │   └── PersonModel.cs           # Khai báo lớp PersonModel với thuộc tính Name và Age
│   │
│   ├── Views/
│   │   ├── Home/
│   │   │   ├── Index.cshtml
│   │   │   ├── Privacy.cshtml
│   │   │   └── NameList.cshtml      # Nhận Model và hiển thị danh sách người dùng
│   │   │
│   │   └── Shared/
│   │       └── _Layout.cshtml       # Thanh điều hướng tích hợp liên kết NameList
│   │
│   ├── Properties/
│   │   └── launchSettings.json      # Cấu hình cổng mạng và môi trường phát triển
│   └── Program.cs                   # Cấu hình dịch vụ và HTTP request pipeline
│
├── .gitattributes
├── .gitignore
└── WebTestOne.slnx
```

---

## Hướng dẫn sử dụng

### 1. Khởi chạy trên máy cục bộ bằng Visual Studio

1. Sao chép mã nguồn về máy:
   ```bash
   git clone [https://github.com/anhtuank9/BuiTuanAnh.Test.git](https://github.com/anhtuank9/BuiTuanAnh.Test.git)
   ```
2. Mở file giải pháp `WebTestOne.slnx` bằng Visual Studio.
3. Nhấn tổ hợp phím **Ctrl + F5** (hoặc nút Run) để biên dịch và chạy website.
4. Mở trình duyệt và truy cập đường dẫn:
   ```text
   http://localhost:5xxx/Home/NameList
   ```
### 2. Khởi chạy trực tuyến bằng GitHub Codespaces

1. Mở cửa sổ Terminal trong Codespaces và chuyển vào thư mục dự án:
   ```bash
   cd WebTestOne
   ```
2. Chạy lệnh khởi động ứng dụng:
   ```bash
   dotnet run
   ```
3. Chọn tab **Ports** (Cổng) ở bảng điều khiển bên dưới, nhấp chuột phải vào cổng đang chạy và đổi **Port Visibility** sang **Public**.
