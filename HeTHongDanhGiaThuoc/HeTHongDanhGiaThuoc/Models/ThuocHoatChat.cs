using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class ThuocHoatChat
    {
        [Key]
        public int MaThuocHoatChat { get; set; }

        [Display(Name = "Thuốc")]
        public int MaThuoc { get; set; }

        [Display(Name = "Hoạt chất")]
        public int MaHoatChat { get; set; }

        [MaxLength(100)]
        [Display(Name = "Hàm lượng")]
        public string? HamLuong { get; set; }

        [ForeignKey("MaThuoc")]
        public Thuoc? Thuoc { get; set; }

        [ForeignKey("MaHoatChat")]
        public HoatChat? HoatChat { get; set; }
    }
}
