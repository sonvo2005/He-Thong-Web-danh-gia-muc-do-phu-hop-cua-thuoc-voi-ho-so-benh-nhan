using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class ThuocHoatChat
    {
        [Key]
        public int MaThuocHoatChat { get; set; }

        [Required]
        [Display(Name = "Thuốc")]
        public int MaThuoc { get; set; } //Khóa ngoại

        [Required]
        [Display(Name = "Hoạt chất")]
        public int MaHoatChat { get; set; } //Khóa ngoại

        [MaxLength(100)]
        [Display(Name = "Hàm lượng")]
        public string? HamLuong { get; set; }

        //Truy vấn khóa ngoại
        [ForeignKey("MaThuoc")]
        public Thuoc? Thuoc { get; set; }

        [ForeignKey("MaHoatChat")]
        public HoatChat? HoatChat { get; set; }
    }
}
