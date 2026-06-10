using System.ComponentModel.DataAnnotations;

namespace HeTHongDanhGiaThuoc.Models
{
    public class HoatChat
    {
        [Key]
        public int MaHoatChat { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Tên hoạt chất")]
        public string TenHoatChat { get; set; } = string.Empty;

        public ICollection<ThuocHoatChat> ThuocHoatChats { get; set; } = new List<ThuocHoatChat>();
        public ICollection<DiUngThuoc> DiUngThuocs { get; set; } = new List<DiUngThuoc>();
    }
}
