using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class ChiTietDanhGia
    {
        [Key]
        public int MaChiTietDanhGia { get; set; }

        [Display(Name = "Đánh giá")]
        public int MaDanhGia { get; set; }

        [Display(Name = "Thuốc")]
        public int MaThuoc { get; set; }

        [MaxLength(255)]
        [Display(Name = "Liều dùng chỉ định")]
        public string? LieuDungChiDinh { get; set; }

        [Display(Name = "Có phù hợp không")]
        public bool CoPhuHopKhong { get; set; } = true;

        [Display(Name = "Lý do không phù hợp")]
        public string? LyDoKhongPhuHop { get; set; }

        [ForeignKey("MaDanhGia")]
        public DanhGia? DanhGia { get; set; }

        [ForeignKey("MaThuoc")]
        public Thuoc? Thuoc { get; set; }
    }
}
