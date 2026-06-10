using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class BenhNen
    {
        [Key]
        public int MaBenhNen { get; set; }

        [Display(Name = "Bệnh nhân")]
        public int? MaBenhNhan { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Tên bệnh nền")]
        public string TenBenhNen { get; set; } = string.Empty;

        [Display(Name = "Ngày chẩn đoán")]
        [DataType(DataType.Date)]
        public DateTime? NgayChanDoan { get; set; }

        [ForeignKey("MaBenhNhan")]
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }
    }
}
