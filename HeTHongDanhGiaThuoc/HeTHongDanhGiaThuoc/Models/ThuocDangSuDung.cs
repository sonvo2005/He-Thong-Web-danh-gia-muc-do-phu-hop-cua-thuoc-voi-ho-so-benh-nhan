using System.ComponentModel.DataAnnotations;
namespace HeTHongDanhGiaThuoc.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class ThuocDangSuDung
{
    [Key]
    public int Id { get; set; }

    public int MaBenhNhan { get; set; }

    public int MaThuoc { get; set; }

    [ForeignKey(nameof(MaBenhNhan))]
    public HoSoBenhNhan? HoSoBenhNhan { get; set; }

    [ForeignKey(nameof(MaThuoc))]
    public Thuoc? Thuoc { get; set; }
}