using System.ComponentModel.DataAnnotations;
namespace HeTHongDanhGiaThuoc.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class ThuocDangSuDung
{
    [Key]
    public int Id { get; set; }

    public int MaBenhNhan { get; set; } //Khóa ngoại

    public int MaThuoc { get; set; }//khóa ngoại

    [ForeignKey(nameof(MaBenhNhan))]
    public HoSoBenhNhan? HoSoBenhNhan { get; set; }

    [ForeignKey(nameof(MaThuoc))]
    public Thuoc? Thuoc { get; set; }

    [Display(Name = "Liều dùng hiện tại")]
    public string? LieuDung { get; set; }

    [Display(Name = "Ngày bắt đầu sử dụng")]
    public DateTime? NgayBatDau { get; set; }
}