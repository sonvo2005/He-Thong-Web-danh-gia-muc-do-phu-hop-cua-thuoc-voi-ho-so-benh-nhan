using System.ComponentModel.DataAnnotations;

namespace HeTHongDanhGiaThuoc.Models
{
    public class Thuoc
    {
        [Key]
        public int MaThuoc { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Tên thuốc")]
        public string TenThuoc { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Tên biệt dược")]
        public string? TenBietDuoc { get; set; }

        [MaxLength(100)]
        [Display(Name = "Dạng bào chế")]
        public string? DangBaoChe { get; set; }

        [MaxLength(255)]
        [Display(Name = "Liều dùng chuẩn")]
        public string? LieuDungChuan { get; set; }

        [MaxLength(50)]
        [Display(Name = "Tình trạng thuốc")]
        public string TinhTrangThuoc { get; set; } = "Hoạt động";

        [Display(Name = "Mô tả chi tiết")]
        public string? MoTaChiTiet { get; set; }

        public ICollection<ThuocHoatChat> ThuocHoatChats { get; set; } = new List<ThuocHoatChat>();
        public ICollection<ChongChiDinh> ChongChiDinhs { get; set; } = new List<ChongChiDinh>();
        public ICollection<ChiTietDanhGia> ChiTietDanhGias { get; set; } = new List<ChiTietDanhGia>();

        public ICollection<TuongTacThuoc> TuongTacA { get; set; } = new List<TuongTacThuoc>();
        public ICollection<TuongTacThuoc> TuongTacB { get; set; } = new List<TuongTacThuoc>();
    }
}
