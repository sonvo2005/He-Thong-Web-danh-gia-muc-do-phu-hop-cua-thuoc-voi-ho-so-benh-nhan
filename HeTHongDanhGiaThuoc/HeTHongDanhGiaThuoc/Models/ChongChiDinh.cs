using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class ChongChiDinh
    {
        [Key]
        public int MaChongChiDinh { get; set; }

        [Required]
        public int MaThuoc { get; set; }

        [Required]
        public int MaBenhNen { get; set; }

        [MaxLength(50)]
        [Display(Name = "Mức độ nguy hiểm")]
        public string? MucDoNguyHiem { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [ForeignKey(nameof(MaThuoc))]
        public Thuoc? Thuoc { get; set; }

        [ForeignKey(nameof(MaBenhNen))]
        public BenhNen? BenhNen { get; set; }
    }
}