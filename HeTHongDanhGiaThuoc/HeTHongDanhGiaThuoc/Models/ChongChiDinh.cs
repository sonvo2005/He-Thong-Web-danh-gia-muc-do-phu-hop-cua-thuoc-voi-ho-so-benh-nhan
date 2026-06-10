using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class ChongChiDinh
    {
        [Key]
        public int MaChongChiDinh { get; set; }

        [Display(Name = "Thuốc")]
        public int MaThuoc { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Tên bệnh chống chỉ định")]
        public string TenBenhChongChiDinh { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [ForeignKey("MaThuoc")]
        public Thuoc? Thuoc { get; set; }
    }
}
