using System.ComponentModel.DataAnnotations;

namespace HeTHongDanhGiaThuoc.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool GhiNho { get; set; }
    }

    public class DanhGiaTaoViewModel
    {
        public int MaBenhNhan { get; set; }
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }
        public List<int> DanhSachThuocIds { get; set; } = new();
        public List<string> LieuDungChiDinhs { get; set; } = new();
        public string? GhiChu { get; set; }
    }

    public class KetQuaDanhGiaViewModel
    {
        public DanhGia? DanhGia { get; set; }
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }
        public List<ChiTietKetQua> ChiTietKetQuas { get; set; } = new();
        public decimal TyLePhuHop { get; set; }
        public List<string> CanhBaos { get; set; } = new();
        public string KhuyenNghi { get; set; } = string.Empty;
    }

    public class ChiTietKetQua
    {
        public Thuoc? Thuoc { get; set; }
        public string? LieuDungChiDinh { get; set; }
        public bool PhuHop { get; set; }
        public List<string> LyDo { get; set; } = new();
    }

    public class NguoiDungViewModel
    {
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Display(Name = "Mật khẩu (để trống nếu không đổi)")]
        [DataType(DataType.Password)]
        public string? MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò")]
        public int MaVaiTro { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThaiHoatDong { get; set; } = true;
    }
}
