using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class ChiTietDanhGia
    {
        [Key]
        public int MaChiTietDanhGia { get; set; }

        public int MaDanhGia { get; set; }

        public int MaThuoc { get; set; }

        // vẫn giữ lại để hiển thị
        public string? LieuDungChiDinh { get; set; }

        // ===================
        // dữ liệu đánh giá
        // ===================

        [Display(Name = "Liều mỗi lần (mg)")]
        public decimal? LieuMoiLanMg { get; set; }

        [Display(Name = "Số lần mỗi ngày")]
        public int? SoLanMoiNgay { get; set; }

        [Display(Name = "Số ngày điều trị")]
        public int? SoNgayDieuTri { get; set; }

        [Display(Name = "Tổng liều mỗi ngày")]
        public decimal? TongLieuNgayMg { get; set; }

        public bool CoPhuHopKhong { get; set; } = true;

        public string? LyDoKhongPhuHop { get; set; }

        public decimal? DiemPhuHop { get; set; }

        public string? CanhBao { get; set; }

        public string? KhuyenNghi { get; set; }

        [ForeignKey(nameof(MaDanhGia))]
        public DanhGia? DanhGia { get; set; }

        [ForeignKey(nameof(MaThuoc))]
        public Thuoc? Thuoc { get; set; }
    }
}
