using System.ComponentModel.DataAnnotations;

namespace HeTHongDanhGiaThuoc.Models
{
    public class HoatChat
    {
        [Key]
        public int MaHoatChat { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Tên hoạt chất")]
        public string TenHoatChat { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        //Truy vấn khóa ngoại
        public ICollection<ThuocHoatChat> ThuocHoatChats { get; set; } = new List<ThuocHoatChat>();

        public ICollection<DiUngThuoc> DiUngThuocs { get; set; } = new List<DiUngThuoc>();
    }
}
