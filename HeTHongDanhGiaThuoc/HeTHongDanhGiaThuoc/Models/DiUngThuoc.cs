using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class DiUngThuoc
    {
        [Key]
        public int MaDiUng { get; set; }

        [Display(Name = "Bệnh nhân")]
        public int? MaBenhNhan { get; set; }

        [Display(Name = "Hoạt chất")]
        public int? MaHoatChat { get; set; }

        [MaxLength(50)]
        [Display(Name = "Mức độ dị ứng")]
        public string? MucDoDiUng { get; set; }

        [ForeignKey("MaBenhNhan")]
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }

        [ForeignKey("MaHoatChat")]
        public HoatChat? HoatChat { get; set; }
    }
}
