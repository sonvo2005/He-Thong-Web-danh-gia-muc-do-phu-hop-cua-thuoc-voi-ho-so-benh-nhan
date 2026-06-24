using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class BenhNhanBenhNen
    {
        [Key]
        public int Id { get; set; } //Khóa chính database

        public int MaBenhNhan { get; set; } //Khóa ngoại

        public int MaBenhNen { get; set; }//Khóa ngoại

        public DateTime? NgayChanDoan { get; set; }

        public string? GhiChu { get; set; }

        //Truy vấn kháo ngoại

        [ForeignKey(nameof(MaBenhNhan))]
        public HoSoBenhNhan? HoSoBenhNhan { get; set; }

        [ForeignKey(nameof(MaBenhNen))]
        public BenhNen? BenhNen { get; set; }
    }
}