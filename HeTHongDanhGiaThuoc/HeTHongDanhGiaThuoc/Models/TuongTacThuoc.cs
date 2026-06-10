using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class TuongTacThuoc
    {
        [Key]
        public int MaTuongTac { get; set; }

        [Display(Name = "Thuốc A")]
        public int MaThuoc_A { get; set; }

        [Display(Name = "Thuốc B")]
        public int MaThuoc_B { get; set; }

        [MaxLength(50)]
        [Display(Name = "Mức độ nghiêm trọng")]
        public string? MucDoNghiemTrong { get; set; }

        [Display(Name = "Mô tả tương tác")]
        public string? MoTaTuongTac { get; set; }

        [ForeignKey("MaThuoc_A")]
        public Thuoc? ThuocA { get; set; }

        [ForeignKey("MaThuoc_B")]
        public Thuoc? ThuocB { get; set; }
    }
}
