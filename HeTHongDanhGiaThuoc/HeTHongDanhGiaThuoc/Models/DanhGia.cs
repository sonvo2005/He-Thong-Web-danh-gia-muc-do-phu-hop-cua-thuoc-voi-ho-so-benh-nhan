using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        [Required]
        [Display(Name = "Bệnh nhân")]
        public int MaBenhNhan { get; set; }

        [Required]
        [Display(Name = "Người đánh giá")]
        public int MaNguoiDanhGia { get; set; }

        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        [Display(Name = "Tỷ lệ phù hợp (%)")]
        [Range(0, 100)]
        public decimal? TyLePhuHop { get; set; }

        [Display(Name = "Cảnh báo y tế")]
        public string? CanhBaoYTe { get; set; }

        [Display(Name = "Khuyến nghị sử dụng")]
        public string? KhuyenNghiSuDung { get; set; }

        [Display(Name = "Ghi chú phiên đánh giá")]
        public string? GhiChuPhienDanhGia { get; set; }

        [ForeignKey("MaBenhNhan")]
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }

        [ForeignKey("MaNguoiDanhGia")]
        public NguoiDung? NguoiDanhGia { get; set; }

        public ICollection<ChiTietDanhGia> ChiTietDanhGias { get; set; } = new List<ChiTietDanhGia>();


        [MaxLength(100)]
        [Display(Name = "Kết luận")]
        public string? KetLuan { get; set; }
    }
}
