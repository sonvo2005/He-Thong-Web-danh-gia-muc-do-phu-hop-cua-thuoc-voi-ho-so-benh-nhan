using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class DiUngThuoc
    {
        [Key]
        public int MaDiUng { get; set; }

        [Required]
        [Display(Name = "Bệnh nhân")]
        public int MaBenhNhan { get; set; } //Khóa ngoại

        [Required]
        [Display(Name = "Hoạt chất")]
        public int MaHoatChat { get; set; } //Khóa ngoại

        [MaxLength(50)]
        [Display(Name = "Mức độ dị ứng")]
        public string? MucDoDiUng { get; set; }

        //Truy vấn khóa ngoại
        [ForeignKey(nameof(MaBenhNhan))]
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }

        [ForeignKey(nameof(MaHoatChat))]
        public HoatChat? HoatChat { get; set; }
    }
}
