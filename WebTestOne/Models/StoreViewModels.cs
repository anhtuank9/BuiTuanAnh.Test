using System.ComponentModel.DataAnnotations;

namespace WebTestOne.Models;

public sealed class BookDetailViewModel
{
    public required Sach Sach { get; init; }
    public string NhaXuatBan { get; init; } = "Đang cập nhật";
    public string ChuDe { get; init; } = "Đang cập nhật";
    public string TacGia { get; init; } = "Đang cập nhật";
    public string ImageUrl { get; init; } = "/images/new.jpg";
}

public sealed class TopicMenuItemViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int BookCount { get; init; }
}

public sealed class SidebarBookViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = "/images/new.jpg";
    public decimal Price { get; init; }
    public int SoldQuantity { get; init; }
}

public sealed class AdvertisementViewModel
{
    public int Id { get; init; }
    public string CompanyName { get; init; } = "Đối tác Book Online";
    public string ImageUrl { get; init; } = "/images/QC01.gif";
    public string Link { get; init; } = "#";
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public sealed class CartItemViewModel
{
    public int BookId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = "/images/new.jpg";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public sealed class CartViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; init; } = [];
    public int TotalQuantity => Items.Sum(item => item.Quantity);
    public decimal GrandTotal => Items.Sum(item => item.LineTotal);
}

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(50)]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Tên đăng nhập dài từ 3 đến 15 ký tự.")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "Mật khẩu dài từ 6 đến 15 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(10)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Điện thoại")]
    public string? DienThoai { get; set; }

    [StringLength(50)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [Display(Name = "Giới tính nam")]
    public bool GioiTinh { get; set; }
}
