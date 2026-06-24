using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class TuongTacThuoc
    {
        [Key]
        public int MaTuongTac { get; set; }

        [Required]
        [Display(Name = "Thuốc A")]
        public int MaThuocA{ get; set; }

        [Required]
        [Display(Name = "Thuốc B")]
        public int MaThuocB { get; set; }

        [MaxLength(50)]
        [Display(Name = "Mức độ nghiêm trọng")]
        public string? MucDoNghiemTrong { get; set; }

        [Display(Name = "Mô tả tương tác")]
        public string? MoTaTuongTac { get; set; }

        [ForeignKey("MaThuocA")]
        public Thuoc? ThuocA { get; set; }

        [ForeignKey("MaThuocB")]
        public Thuoc? ThuocB { get; set; }
    }
}
