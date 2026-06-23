using System.ComponentModel.DataAnnotations;

namespace HeTHongDanhGiaThuoc.Models
{
    public class VaiTro
    {
        [Key]
        public int MaVaiTro { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Tên vai trò")]
        public string TenVaiTro { get; set; } = string.Empty;

    //Khóa ngoại
        public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }
}
