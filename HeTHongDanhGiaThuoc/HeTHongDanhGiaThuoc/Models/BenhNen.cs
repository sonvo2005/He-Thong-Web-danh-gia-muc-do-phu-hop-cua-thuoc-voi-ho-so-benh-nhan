using System.ComponentModel.DataAnnotations;
using HeTHongDanhGiaThuoc.Models;
namespace HeTHongDanhGiaThuoc.Models
{
    public class BenhNen
    {
        [Key]
        public int MaBenhNen { get; set; }//Khóa chính database

        [Required]
        [MaxLength(150)]
        [Display(Name = "Tên bệnh nền")]
        public string TenBenhNen { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

//Khóa ngoại
        public ICollection<BenhNhanBenhNen> BenhNhanBenhNens { get; set; } = new List<BenhNhanBenhNen>();

        public ICollection<ChongChiDinh> ChongChiDinhs { get; set; }  = new List<ChongChiDinh>();

    }
}