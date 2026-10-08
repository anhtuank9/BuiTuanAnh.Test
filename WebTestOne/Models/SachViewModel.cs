namespace WebTestOne.Models  
{
    public class SachViewModel
    {
        public int Ms { get; set; }

        public string TenSach { get; set; } = null!;

        public decimal? DonGia { get; set; }

        public string? HinhMinhHoa { get; set; }

        public string? MoTa { get; set; }

        // Thuộc tính bổ sung để lấy tên Nhà xuất bản từ bảng kết nối
        public string? TenNXB { get; set; }

        public string? TenChuDe { get; set; }

        public string? TacGia { get; set; }

        public string ImageUrl { get; set; } = "/images/new.jpg";
    }
}
