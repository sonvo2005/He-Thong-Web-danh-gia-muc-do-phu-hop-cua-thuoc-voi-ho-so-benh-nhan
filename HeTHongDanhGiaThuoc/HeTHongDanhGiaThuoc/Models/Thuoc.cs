using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public partial class Thuoc
    {
        [Key]
        public int MaThuoc { get; set; }

        [Required, MaxLength(200)]
        public string TenThuoc { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? TenBietDuoc { get; set; }

        [MaxLength(100)]
        public string? DangBaoChe { get; set; }

        // ==========================
        // LIỀU CƠ BẢN
        // ==========================
        public string? LieuDungChuan { get; set; }

        public decimal? LieuToiThieuMg { get; set; }
        public decimal? LieuToiDaMg { get; set; }

        public int? TanSuatToiDaMoiNgay { get; set; }

        public decimal? LieuKhuyenNghiMg { get; set; }

        // ==========================
        // GIỚI HẠN BỆNH NHÂN
        // ==========================
        public int? TuoiToiThieu { get; set; }
        public int? TuoiToiDa { get; set; }

        public decimal? CanNangToiThieu { get; set; }
        public decimal? CanNangToiDa { get; set; }

        // ==========================
        // THÔNG TIN KHÁC
        // ==========================
        public string TinhTrangThuoc { get; set; } = "Hoạt động";
        public string? MoTaChiTiet { get; set; }
        public string? HuongDanSuDung { get; set; }

        // ==========================
        // NAVIGATION
        // ==========================
        public ICollection<ThuocHoatChat> ThuocHoatChats { get; set; } = new List<ThuocHoatChat>();
        public ICollection<ChongChiDinh> ChongChiDinhs { get; set; } = new List<ChongChiDinh>();
        public ICollection<ChiTietDanhGia> ChiTietDanhGias { get; set; } = new List<ChiTietDanhGia>();
        public ICollection<TuongTacThuoc> TuongTacA { get; set; } = new List<TuongTacThuoc>();
        public ICollection<TuongTacThuoc> TuongTacB { get; set; } = new List<TuongTacThuoc>();

        // ==========================
        // COMPUTED PROPERTIES
        // ==========================
        [NotMapped]
        public decimal TongLieuToiDaNgay
        //VD Tổng Liều Tối Đa Ngày = Liều tối đa/ 1 lần sử dụng  x  Tần Suất tối đa/ ngày
        // Tổng liều tối đa ngày = 1000Mg/ 1 lần sử dụng x 3 lần/ngày
        //Tổng liều tối đa ngày = 3000Mg/ngày
        {
            get
            {
                if (!LieuToiDaMg.HasValue || !TanSuatToiDaMoiNgay.HasValue)
                    return 0;

                return (decimal)LieuToiDaMg.Value * TanSuatToiDaMoiNgay.Value;
            }
        }

        [NotMapped]
        public decimal TongLieuKhuyenNghiNgay
        {  //VD Tổng Liều Khuyến Nghị Ngày = Liều Khuyến Nghị/ 1 lần sử dụng  x  Tần Suất tối đa/ ngày
           // Tổng liều khuyến nghị = 500Mg/ 1 lần sử dụng x 3 lần/ngày
           //Tổng liều khuyến nghị = 1500Mg/ngày
            get
            {
                if (!LieuKhuyenNghiMg.HasValue || !TanSuatToiDaMoiNgay.HasValue)
                    return 0;

                return (decimal)LieuKhuyenNghiMg.Value * TanSuatToiDaMoiNgay.Value;
            }
        }
    }
}