using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [Display(Name = "Mật khẩu")]
        public string MatKhauMaHoa { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Vai trò")]
        public int MaVaiTro { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThaiHoatDong { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("MaVaiTro")]
        public VaiTro? VaiTro { get; set; }

        public ICollection<HoSoBenhNhan> HoSoBenhNhans { get; set; } = new List<HoSoBenhNhan>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
